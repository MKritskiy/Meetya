using Events.Application.Interfaces;
using Events.Domain.Entities;

namespace Events.Infrastructure.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;

    public EventService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<int> AddEvent(Event @event)
    {
        return await _eventRepository.AddAsync(@event) ?? 0;
    }

    public async Task DeleteEvent(int id)
    {
        await _eventRepository.DeleteByIdAsync(id);
    }

    public async Task<Event> GetEvent(int id)
    {
        return await _eventRepository.GetByIdAsync(id) ?? new Event();
    }

    public async Task<IEnumerable<Event>> GetEvents()
    {
        return await _eventRepository.Get();
    }

    public Task<bool> UpdateEvent(Event @event)
    {
        return _eventRepository.UpdateAsync(@event);
    }
}
