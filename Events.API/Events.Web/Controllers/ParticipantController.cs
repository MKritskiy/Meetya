﻿using Events.Application.Interfaces;
using Events.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Events.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParticipantController : ControllerBase
{
    private readonly IParticipantService _participantService;

    public ParticipantController(IParticipantService participantService)
    {
        _participantService = participantService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddParticipantToEvent( [FromBody] EventParticipant participant)
    {
        try
        {
            var result = await _participantService.AddParticipantToEvent(participant);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveParticipantFromEvent([FromBody] EventParticipant participant)
    {
        try
        {
            await _participantService.RemoveParticipantFromEvent(participant);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
