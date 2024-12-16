using Friends.Domain.Entities;
using System.Reflection;

namespace Friends.Infrastructure.Data;

public class FriendDbContext : DbContext
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
}
