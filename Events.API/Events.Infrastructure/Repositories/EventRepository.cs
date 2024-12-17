using Events.Domain.Entities;
using Infrastructure.Repositories;
using Events.Application.Interfaces;
using Events.Infrastructure.Data;
namespace Events.Infrastructure.Repositories;

public class EventRepository : BaseRepository<Event>, IEventRepository
{
    public EventRepository(EventDbContext context) : base(context)
    {
    }

    protected override int? GetId(Event entity)
    {
        return entity.Id;
    }
}
