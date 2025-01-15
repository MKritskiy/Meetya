using Microsoft.AspNetCore.Mvc;
using Users.Application.Interfaces;
using Users.Domain.Entities;

namespace Users.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController(IProfileService ProfileService) : ControllerBase
{
    [HttpPost("add")]
    public async Task<IActionResult> AddProfile([FromBody] Profile profile)
    {
        try
        {
            var profileId = await ProfileService.AddProfile(profile);
            return Ok(profileId);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("delete/{profileId}")]
    public async Task<IActionResult> DeleteProfile(int profileId)
    {
        try
        {
            await ProfileService.DeleteProfile(profileId);
            return Ok();
        }
        catch (InvalidOperationException)
        {
            return NotFound("Profile not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{profileId}")]
    public async Task<ActionResult<Profile?>> GetProfileById(int profileId)
    {
        try
        {
            var profile = await ProfileService.GetProfileById(profileId);
            return Ok(profile);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetProfilesByUserId(int userId)
    {
        try
        {
            var profiles = await ProfileService.GetProfilesByUserId(userId);
            return Ok(profiles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
