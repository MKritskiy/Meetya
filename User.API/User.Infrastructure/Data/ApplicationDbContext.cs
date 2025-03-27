using Infrastructure.Data;
using System.Reflection;
using Users.Domain.Entities;


namespace Users.Infrastructure.Data
{
    public class ApplicationDbContext : AuditableDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }

    }
}
