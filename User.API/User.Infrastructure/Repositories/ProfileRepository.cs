using Microsoft.EntityFrameworkCore;
using Users.Application.Interfaces;
using Users.Domain.Entities;
using Users.Infrastructure.Data;

namespace Users.Infrastructure.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public ProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int?> AddProfileAsync(Profile profile)
        {
            await _context.Profiles.AddAsync(profile);
            await _context.SaveChangesAsync();
            return profile.Id;
        }

        public async Task<bool> DeleteProfileByIdAsync(int profileid)
        {
            var profile = await _context.Profiles.FindAsync(profileid);
            if (profile == null)
            {
                return false;
            }

            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Profile?> GetProfileByIdAsync(int profileid)
        {
            return await _context.Profiles.FindAsync(profileid);
        }

        public async Task<IEnumerable<Profile>> GetProfilesByUserIdAsync(int userid)
        {
            return await _context.Profiles.Where(p => p.UserId == userid).ToListAsync();
        }
    }
}
