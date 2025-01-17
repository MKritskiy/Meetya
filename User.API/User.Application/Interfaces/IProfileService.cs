using Users.Domain.Entities;

namespace Users.Application.Interfaces
{
    public interface IProfileService
    {
        Task<int> AddProfile(Profile profile);
        Task DeleteProfile(int profileId);
        Task<Profile> GetProfileById(int profileId);
        Task<IEnumerable<Profile>> GetProfilesByUserId(int userid);
        Task<IEnumerable<Profile>> GetProfilesByIds(int?[] ids);
    }
}
