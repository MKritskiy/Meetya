using Application.Interfaces;
using Users.Domain.Entities;

namespace Users.Application.Interfaces
{
    public interface IProfileRepository : IBaseRepository<Profile>
    {
        Task<IEnumerable<Profile>> GetProfilesByUserIdAsync(int userid);
    }
}
