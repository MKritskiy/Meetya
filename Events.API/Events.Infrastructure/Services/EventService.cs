using Domain.Constants;
using Events.Application;
using Events.Application.Interfaces;
using Events.Application.Models;
using Events.Domain.Entities;
using System.Net.Http.Json;

namespace Events.Infrastructure.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly HttpClient _httpClient;

    public EventService(IEventRepository eventRepository, HttpClient httpClient)
    {
        _eventRepository = eventRepository;
        _httpClient = httpClient;
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
    //TODO: возвращать EventDto с данными профиля
    public async Task<EventDto> GetEvent(int id)
    {
        
        if (id <= 0) throw new ArgumentException("Invalid event ID.");
        var @event = await _eventRepository.GetByIdAsync(includeProperties: "Polls,EventParticipants", ids: id);
        if (@event == null) throw new KeyNotFoundException("Event not found.");
        var client = new UsersApiClient(GatewayConstants.GATEWAY_INTERNAL_HOST + GatewayConstants.USER_API_ROUTE, _httpClient);
        Profile profile = await client.ProfileAsync(@event.CreatorId);

        EventDto eventDto = new EventDto()
        {
            Event = @event,
            Profile = profile
        };

        return eventDto;
    }
    //TODO: заменить на возврат IEnumerable<EventDto>
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
