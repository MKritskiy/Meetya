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
        if (poll == null) throw new ArgumentNullException(nameof(poll));
        if (!IsValidPoll(poll)) throw new ArgumentException("Invalid poll data.");

        return await _pollRepository.AddAsync(poll) ?? 0;
    }

    public async Task DeletePoll(int pollId)
    {
        if (pollId <= 0) throw new ArgumentException("Invalid poll ID.");
        await _pollRepository.DeleteByIdAsync(pollId);
    }

    public async Task<Poll> GetPollByProfileId(int profileId)
    {
        if (profileId <= 0) throw new ArgumentException("Invalid profile ID.");
        var res = await _pollRepository.Get(filter: p => p.ProfileId == profileId);
        return res.FirstOrDefault() ?? throw new KeyNotFoundException("Poll not found.");
    }

    public async Task<IEnumerable<Poll>> GetPollsByEventId(int eventId)
    {
        if (eventId <= 0) throw new ArgumentException("Invalid event ID.");
        var res = await _pollRepository.Get(includeProperties: "Event", filter: p => p.EventId == eventId);
        return res;
    }

    public async Task<bool> UpdatePoll(Poll poll)
    {
        if (poll == null) throw new ArgumentNullException(nameof(poll));
        if (!IsValidPoll(poll)) throw new ArgumentException("Invalid poll data.");

        return await _pollRepository.UpdateAsync(poll);
    }

    private bool IsValidPoll(Poll poll)
    {
        return poll.PreferredDates != null;
    }
}
