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
        if (@event == null) throw new ArgumentNullException(nameof(@event));
        if (!IsValidEvent(@event)) throw new ArgumentException("Invalid event data.");

        return await _eventRepository.AddAsync(@event) ?? 0;
    }

    public async Task DeleteEvent(int id)
    {
        if (id <= 0) throw new ArgumentException("Invalid event ID.");
        await _eventRepository.DeleteByIdAsync(id);
    }

    public async Task<Event> GetEvent(int id)
    {
        if (id <= 0) throw new ArgumentException("Invalid event ID.");
        var @event = await _eventRepository.GetByIdAsync(includeProperties: "Polls,EventParticipants", ids: id);
        if (@event == null) throw new KeyNotFoundException("Event not found.");
        return @event;
    }

    public async Task<IEnumerable<Event>> GetEvents()
    {
        return await _eventRepository.Get(includeProperties: "Polls,EventParticipants");
    }

    public async Task<bool> UpdateEvent(Event @event)
    {
        if (@event == null) throw new ArgumentNullException(nameof(@event));
        if (!IsValidEvent(@event)) throw new ArgumentException("Invalid event data.");

        return await _eventRepository.UpdateAsync(@event);
    }

    private bool IsValidEvent(Event @event)
    {
        // Здесь должна быть логика проверки полей (нужно будет создать класс валидатор через fluent validation)
        return @event.Name != null && @event.Time != null;
    }
}
