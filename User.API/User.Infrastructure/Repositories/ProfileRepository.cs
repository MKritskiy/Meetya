using Infrastructure.Repositories;
using Users.Application.Interfaces;
using Users.Domain.Entities;
using Users.Infrastructure.Data;

namespace Users.Infrastructure.Repositories
{
    public class ProfileRepository : BaseRepository<Profile>, IProfileRepository
    {
        public ProfileRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Profile>> GetProfilesByUserIdAsync(int userid)
        {
            return await _dbSet.Where(p => p.UserId == userid).ToListAsync();
        }

        protected override int? GetId(Profile entity)
        {
            return entity.Id;
        }
    }
}
