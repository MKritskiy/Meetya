using Application.Interfaces;
using Users.Domain.Entities;

namespace Users.Application.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetUserByEmailAsync(string email);
    }
}
