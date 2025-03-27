using Microsoft.AspNetCore.Mvc;
using Events.Domain.Entities;
using Events.Application.Interfaces;
using Events.Application.Models;

namespace Events.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly IEventRepository _eventRepository;

    public EventController(IEventService eventService, IEventRepository eventRepository)
    {
        _eventService = eventService;
        _eventRepository = eventRepository;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddEvent([FromBody] AddEventDto addEventDto)
    {
        try
        {
            var eventId = await _eventService.AddEvent(addEventDto.ToEvent());
            return Ok(eventId);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        try
        {
            await _eventService.DeleteEvent(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EventDto>> GetEvent(int id)
    {
        try
        {

            EventDto eventDto = await _eventService.GetEvent(id);
            return Ok(eventDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents()
    {
        try
        {
            var events = await _eventService.GetEvents();
            return Ok(events);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateEvent([FromBody] AddEventDto addEventDto, int id)
    {
        try
        {
            var @event = await _eventRepository.GetByIdAsync(id);
            var result = await _eventService.UpdateEvent(addEventDto.UpdateEventFields(@event));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
