using Events.Domain.Entities;
using Infrastructure.Data;

namespace Events.Infrastructure.Data;

public class EventDbContext : AuditableDbContext
{
    public EventDbContext(DbContextOptions<EventDbContext> options) : base(options) { }

    public DbSet<Event> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<EventParticipant>()
            .HasKey(ep => new { ep.EventId, ep.ProfileId});
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
