using Users.Application.Models;
using Users.Domain.Entities;

namespace Users.Application.Interfaces
{
    public interface IUserService
    {
        Task ValidateEmail(string email);
        Task Register(RegDto regDto);
        Task<AfterAuthDto> Login(LoginDto loginDto);
        Task<int> CreateUser(User user);
        Task<int> UpdateUser(int userId, string phoneNumber, string password);
        Task<int> DeleteUser(int userId);
        Task GenerateAndSendConfiramtionCode(string? email = null, string? phone = null);
        Task<AfterAuthDto> ConfirmEmail(string email, string code);

    }
}
