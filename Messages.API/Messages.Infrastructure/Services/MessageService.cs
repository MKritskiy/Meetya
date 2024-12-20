using Messages.Application.Interfaces;
using Messages.Domain.Entities;

namespace Messages.Infrastructure.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;

    public MessageService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<int> AddMessage(Message message)
    {
        return await _messageRepository.AddAsync(message) ?? 0;
    }

    public async Task DeleteMessage(int id)
    {
        await _messageRepository.DeleteByIdAsync(id);
    }

    public async Task<Message> GetMessage(int id)
    {
        return await _messageRepository.GetByIdAsync(id) ?? new Message();
    }

    public async Task<IEnumerable<Message>> GetMessagesByEventId(int eventId, int countInPage, int page)
    {
        return await _messageRepository.Get(
            m => m.EventId == eventId, 
            q => q.OrderByDescending(m => m.Id), 
            page: page, 
            pageSize: countInPage);
    }

    public async Task<IEnumerable<Message>> GetMessagesByProfileId(int profileId, int eventId)
    {
        return await _messageRepository.Get(m => m.ProfileId == profileId && m.EventId == eventId, q=> q.OrderByDescending(m=>m.Id));
    }

    public async Task<bool> UpdateMessage(Message message)
    {
        return await _messageRepository.UpdateAsync(message);
    }
}
