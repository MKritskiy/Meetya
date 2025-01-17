using Events.Application.Models;
using Events.Domain.Entities;

namespace Events.Application.Interfaces;

public interface IEventService
{
    Task<int> AddEvent(Event @event);
    Task DeleteEvent(int id);
    Task<EventDto> GetEvent(int id);
    Task<IEnumerable<EventDto>> GetEvents();
    Task<bool> UpdateEvent(Event @event);
}
