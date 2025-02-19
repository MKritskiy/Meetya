using Domain.Constants;
using Events.Application;
using Events.Application.Interfaces;
using Events.Application.Models;
using Events.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Events.Infrastructure.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IUsersApiClient _usersApiClient;
    private readonly ILogger<EventService> _logger;

    public EventService(IEventRepository eventRepository, IUsersApiClient usersApiClient, ILogger<EventService> logger)
    {
        _eventRepository = eventRepository;
        _usersApiClient = usersApiClient;
        _logger = logger;
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
    public async Task<EventDto> GetEvent(int id)
    {
        
        if (id <= 0) throw new ArgumentException("Invalid event ID.");
        var @event = await _eventRepository.GetByIdAsync(includeProperties: "Polls,EventParticipants", ids: id);
        if (@event == null) throw new KeyNotFoundException("Event not found.");
        try
        {
            Profile profile = await _usersApiClient.ProfileAsync(@event.CreatorId);
            return new EventDto
            {
                Event = @event,
                Profile = profile
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error fetching profile for event ID {EventId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<EventDto>> GetEvents()
    {

        var events = await _eventRepository.Get(includeProperties: "Polls,EventParticipants");
        var profileIds = events.Select(e=>e.CreatorId).Distinct();

        if (profileIds.Any())
        {
            var profiles = await _usersApiClient.ProfilesAsync(profileIds);

            var profileDictionary = profiles.ToDictionary(p => p.Id ?? 0);

            var eventsDtos = events.Select(e => new EventDto
            {
                Event = e,
                Profile = profileDictionary.ContainsKey(e.CreatorId) ? profileDictionary[e.CreatorId] : null,
            });

            return eventsDtos;
        }
        return events.Select(e=> new EventDto { Event = e, Profile = null });
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
