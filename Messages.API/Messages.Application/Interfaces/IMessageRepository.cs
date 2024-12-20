using Application.Interfaces;
using Messages.Domain.Entities;

namespace Messages.Application.Interfaces;

public interface IMessageRepository : IBaseRepository<Message>
{

}
