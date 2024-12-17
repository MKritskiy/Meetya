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

    public async Task<bool> AddParticipantToEvent(EventParticipant participant, int eventId)
    {
        return (await _eventParticipantRepository.AddAsync(participant) ?? 0) > 0;
    }

    public async Task RemoveParticipantFromEvent(EventParticipant participant, int eventId)
    {
        await _eventParticipantRepository.DeleteByIdAsync(participant.EventId, participant.ProfileId);
    }
}
