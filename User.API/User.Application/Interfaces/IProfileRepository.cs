using Users.Domain.Entities;

namespace Users.Application.Interfaces
{
    public interface IProfileRepository
    {
        Task<Profile?> GetProfileByIdAsync(int profileid);
        Task<bool> DeleteProfileByIdAsync(int profileid);
        Task<int?> AddProfileAsync(Profile profile);
        Task<IEnumerable<Profile>> GetProfilesByUserIdAsync(int userid);
    }
}
