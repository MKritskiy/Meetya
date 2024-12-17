using Events.Domain.Entities;

namespace Events.Application.Interfaces;

public interface IParticipantService
{
    Task<bool> AddParticipantToEvent(EventParticipant participant, int eventId);
    Task RemoveParticipantFromEvent(EventParticipant participant, int eventId);
}
