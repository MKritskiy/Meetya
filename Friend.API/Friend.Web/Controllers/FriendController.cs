using Friends.Application.Interfaces;
using Friends.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Friends.Web.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FriendController(IFriendService FriendService) : ControllerBase
{

    [HttpPost("add")]
    public async Task<IActionResult> AddFriend([FromBody] Friend friend)
    {
        try
        {
            var friendId = await FriendService.AddFriend(friend);
            return Ok(friendId);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("delete/{profileId}/{friendId}")]
    public async Task<IActionResult> DeleteFriend(int profileId, int friendId)
    {
        try
        {
            await FriendService.DeleteFriend(profileId, friendId);
            return Ok();
        }
        catch (InvalidOperationException)
        {
            return NotFound("Friend not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{profileId}/first_friend")]
    public async Task<IActionResult> GetFirstFriendByProfileId(int profileId)
    {
        try
        {
            var friend = await FriendService.GetFriendByProfileId(profileId);
            return Ok(friend);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("profile/{profileId}")]
    public async Task<IActionResult> GetFriendsByProfileId(int profileId)
    {
        try
        {
            var friends = await FriendService.GetFriendsByProfileId(profileId);
            return Ok(friends);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}

