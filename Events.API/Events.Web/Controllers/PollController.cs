using Events.Application.Interfaces;
using Events.Application.Models;
using Events.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Events.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PollController : ControllerBase
{
    private readonly IPollService _pollService;

    public PollController(IPollService pollService)
    {
        _pollService = pollService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddPoll([FromBody] AddPollDto addPollDto)
    {
        try
        {
            var pollId = await _pollService.AddPoll(addPollDto.ToPoll());
            return Ok(pollId);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("delete/{pollId}")]
    public async Task<IActionResult> DeletePoll(int pollId)
    {
        try
        {
            await _pollService.DeletePoll(pollId);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("profile/{profileId}")]
    public async Task<IActionResult> GetPollByProfileId(int profileId)
    {
        try
        {
            var poll = await _pollService.GetPollByProfileId(profileId);
            return Ok(poll);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> GetPollsByEventId(int eventId)
    {
        try
        {
            var polls = await _pollService.GetPollsByEventId(eventId);
            return Ok(polls);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdatePoll([FromBody] Poll poll)
    {
        try
        {
            var result = await _pollService.UpdatePoll(poll);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}

