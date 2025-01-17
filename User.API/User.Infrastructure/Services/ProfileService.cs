using Users.Application.Interfaces;
using Users.Domain.Entities;

namespace Users.Infrastructure.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;

        public ProfileService(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<int> AddProfile(Profile profile)
        {
            return await _profileRepository.AddAsync(profile) ?? 0;
        }

        public async Task DeleteProfile(int profileId)
        {
            bool res = await _profileRepository.DeleteByIdAsync(profileId);
            if (!res) throw new InvalidOperationException();
        }

        public async Task<Profile> GetProfileById(int profileId)
        {
            return await _profileRepository.GetByIdAsync(profileId) ?? new Profile();
        }

        public async Task<IEnumerable<Profile>> GetProfilesByUserId(int userid)
        {
            return await _profileRepository.GetProfilesByUserIdAsync(userid);
        }

        public async Task<IEnumerable<Profile>> GetProfilesByIds(int?[] ids)
        {
            var idSet = new HashSet<int?>(ids);
            return await _profileRepository.Get(p => idSet.Contains(p.Id));
        }
    }
}
