using Events.Domain.Entities;

namespace Events.Application.Interfaces;

public interface IParticipantService
{
    Task<bool> AddParticipantToEvent(EventParticipant participant);
    Task RemoveParticipantFromEvent(EventParticipant participant);
}
