using Events.Application.Interfaces;
using Events.Domain.Entities;

namespace Events.Infrastructure.Services;

public class PollService : IPollService
{
    private readonly IPollRepository _pollRepository;

    public PollService(IPollRepository pollRepository)
    {
        _pollRepository = pollRepository;
    }

    public async Task<int> AddPoll(Poll poll)
    {
        return await _pollRepository.AddAsync(poll) ?? 0;
    }

    public async Task DeletePoll(int pollId)
    {
        await _pollRepository.DeleteByIdAsync(pollId);
    }

    public async Task<Poll> GetPollByProfileId(int profileId)
    {
        var res = await _pollRepository.Get(filter: p => p.ProfileId == profileId);
        return res.FirstOrDefault()??new Poll();
    }

    public async Task<IEnumerable<Poll>> GetPollsByEventId(int eventId)
    {
        var res = await _pollRepository.Get(filter: p=> p.EventId== eventId);
        return res;
    }

    public async Task<bool> UpdatePoll(Poll poll)
    {
        return await _pollRepository.UpdateAsync(poll);
    }
}
