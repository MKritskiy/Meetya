using Messages.Application.Interfaces;
using Messages.Application.Models;
using Messages.Application.Users;
using Messages.Domain.Entities;

namespace Messages.Infrastructure.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUsersApiClient _usersApiClient;

    public MessageService(IMessageRepository messageRepository, IUsersApiClient usersApiClient)
    {
        _messageRepository = messageRepository;
        _usersApiClient = usersApiClient;
    }

    public async Task<int> AddMessage(Message message)
    {
        return await _messageRepository.AddAsync(message) ?? 0;
    }

    public async Task DeleteMessage(int id)
    {
        await _messageRepository.DeleteByIdAsync(id);
    }

    public async Task<MessageDto> GetMessage(int id)
    {
        var res = new MessageDto()
        {
            Message = await _messageRepository.GetByIdAsync(id) ?? new Message(),
            Profile = await _usersApiClient.ApiProfileAsync(id),
        };
        return res;
    }

    public async Task<IEnumerable<MessageDto>> GetMessagesByEventId(int eventId, int countInPage, int page)
    {
        var messages = await _messageRepository.Get(
            m => m.EventId == eventId,
            q => q.OrderByDescending(m => m.Id),
            page: page,
            pageSize: countInPage);
        var profileIds = messages.Select(m => m.ProfileId).Distinct();
        var profiles = await _usersApiClient.ApiProfileProfilesAsync(profileIds);
        var profileDictionary = profiles.ToDictionary(p => p.Id ?? 0);
        var res = messages.Select(m => new MessageDto()
        {
            Message = m,
            Profile = profileDictionary.ContainsKey(m.ProfileId) ? profileDictionary[m.ProfileId] : null,
        });
        return res;
    }

    public async Task<IEnumerable<MessageDto>> GetMessagesByProfileId(int profileId, int eventId)
    {
        var messages = await _messageRepository.Get(m => m.ProfileId == profileId && m.EventId == eventId, q => q.OrderByDescending(m => m.Id));
        var profile = await _usersApiClient.ApiProfileAsync(profileId);
        var res = messages.Select(m=> new MessageDto() { Message = m, Profile = profile });
        return res;
    }

    public async Task<bool> UpdateMessage(Message message)
    {
        return await _messageRepository.UpdateAsync(message);
    }
}
