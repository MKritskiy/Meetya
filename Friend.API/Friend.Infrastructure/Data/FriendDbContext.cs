using Friends.Domain.Entities;
using Infrastructure.Data;
using System.Reflection;

namespace Friends.Infrastructure.Data;

public class FriendDbContext : AuditableDbContext
{
    public FriendDbContext(DbContextOptions<FriendDbContext> options) : base(options) { }

    public DbSet<Friend> Friends { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Friend>()
            .HasKey(f => new { f.ProfileId, f.FriendId });
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void SetAuditFields()
    {
        var entries = ChangeTracker.Entries<Friend>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.Created = DateTimeOffset.UtcNow;
            }
        }
    }

}
