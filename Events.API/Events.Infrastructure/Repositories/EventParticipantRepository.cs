using Events.Application.Interfaces;
using Events.Domain.Entities;
using Events.Infrastructure.Data;
using Infrastructure.Repositories;

namespace Events.Infrastructure.Repositories;

public class EventParticipantRepository : BaseRepository<EventParticipant>, IEventParticipantRepository
{
    public EventParticipantRepository(EventDbContext context) : base(context)
    {
    }

    protected override int? GetId(EventParticipant entity)
    {
        return entity.EventId;
    }
}
