using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Users.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("This is a public endpoint.");
        }

        [Authorize]
        [HttpGet("protected")]
        public IActionResult Protected()
        {
            return Ok("This is a protected endpoint.");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult Admin()
        {
            return Ok("This is an admin endpoint.");
        }
    }
}
