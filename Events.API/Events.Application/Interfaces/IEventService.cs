using Events.Domain.Entities;

namespace Events.Application.Interfaces;

public interface IEventService
{
    Task<int> AddEvent(Event @event);
    Task DeleteEvent(int id);
    Task<Event> GetEvent(int id);
    Task<IEnumerable<Event>> GetEvents();
    Task<bool> UpdateEvent(Event @event);
}
