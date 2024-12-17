using Events.Application.Interfaces;
using Events.Domain.Entities;

namespace Events.Infrastructure.Services;

public class ParticipantService : IParticipantService
{
    private readonly IEventParticipantRepository _eventParticipantRepository;

    public ParticipantService(IEventParticipantRepository eventParticipantRepository)
    {
        _eventParticipantRepository = eventParticipantRepository;
    }

    public async Task<bool> AddParticipantToEvent(EventParticipant participant)
    {
        if (participant == null) throw new ArgumentNullException(nameof(participant));
        if (!IsValidParticipant(participant)) throw new ArgumentException("Invalid participant data.");

        return (await _eventParticipantRepository.AddAsync(participant) ?? 0) > 0;
    }

    public async Task RemoveParticipantFromEvent(EventParticipant participant)
    {
        if (participant == null) throw new ArgumentNullException(nameof(participant));

        await _eventParticipantRepository.DeleteByIdAsync(participant.EventId, participant.ProfileId);
    }

    private bool IsValidParticipant(EventParticipant participant)
    {
        return participant.ProfileId > 0 && participant.EventId > 0;
    }
}
