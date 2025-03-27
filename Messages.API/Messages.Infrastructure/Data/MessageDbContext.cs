using Infrastructure.Data;
using Messages.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Messages.Infrastructure.Data;

public class MessageDbContext : AuditableDbContext
{
    public MessageDbContext(DbContextOptions<MessageDbContext> options) : base(options) { }

    public DbSet<Message> Messages { get; set; }


}
