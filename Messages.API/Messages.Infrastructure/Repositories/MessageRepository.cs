using Infrastructure.Repositories;
using Messages.Domain.Entities;
using Messages.Application.Interfaces;
using Messages.Infrastructure.Data;

namespace Messages.Infrastructure.Repositories;

public class MessageRepository : BaseRepository<Message>, IMessageRepository
{
    public MessageRepository(MessageDbContext context) : base(context)
    {
    }

    protected override int? GetId(Message entity)
    {
        return entity.Id;
    }
}
