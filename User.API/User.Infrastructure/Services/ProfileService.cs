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
            return await _profileRepository.AddProfileAsync(profile) ?? 0;
        }

        public async Task DeleteProfile(int profileId)
        {
            bool res = await _profileRepository.DeleteProfileByIdAsync(profileId);
            if (!res) throw new InvalidOperationException();
        }

        public async Task<Profile> GetProfileById(int profileId)
        {
            return await _profileRepository.GetProfileByIdAsync(profileId) ?? new Profile();
        }

        public async Task<IEnumerable<Profile>> GetProfilesByUserId(int userid)
        {
            return await _profileRepository.GetProfilesByUserIdAsync(userid);
        }
    }
}
