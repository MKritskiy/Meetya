using Users.Domain.Entities;

namespace Users.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int userid);
        Task<User?> GetUserByEmailAsync(string email);
        Task<int?> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
    }
}
