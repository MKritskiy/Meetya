using Messages.Application.Users;
using Messages.Domain.Entities;
namespace Messages.Application.Models;

public class MessageDto
{
    public Message? Message { get; set; }
    public Profile? Profile { get; set; }
}
