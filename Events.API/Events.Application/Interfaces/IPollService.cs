using Events.Domain.Entities;

namespace Events.Application.Interfaces;

public interface IPollService
{
    Task<int> AddPoll(Poll poll);
    Task<Poll> GetPollByProfileId(int profileId);
    Task<IEnumerable<Poll>> GetPollsByEventId(int eventId);
    Task DeletePoll(int pollId);
    Task<bool> UpdatePoll(Poll poll);
}
