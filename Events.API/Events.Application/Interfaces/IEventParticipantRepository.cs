using Application.Interfaces;
using Events.Domain.Entities;

namespace Events.Application.Interfaces;

public interface IEventParticipantRepository : IBaseRepository<EventParticipant>
{
}
