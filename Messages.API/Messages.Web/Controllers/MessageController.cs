using Messages.Application.Interfaces;
using Messages.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Messages.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet("{eventId}")]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessages(int eventId, [FromQuery] int page, [FromQuery] int pageSize)
    {
        var res = await _messageService.GetMessagesByEventId(eventId, page, pageSize);
        return Ok(res);
    }


}
