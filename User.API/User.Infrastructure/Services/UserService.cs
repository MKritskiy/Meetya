using Users.Application.Exceptions;
using Users.Application.Interfaces;
using Users.Application.Models;
using Users.Domain.Entities;

namespace Users.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IEncrypt _encrypt;

        public UserService(IUserRepository userRepository, ITokenService tokenService, IEncrypt encrypt)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _encrypt = encrypt;
        }

        public async Task<int> CreateUser(User user)
        {
            if (user.Email == null || user.Password == null) throw new InvalidOperationException("Incorrect User Data");
            user.Salt = General.Helpers.GenerateSalt();
            user.Password = _encrypt.HashPassword(user.Password, user.Salt);
            return await _userRepository.CreateUserAsync(user) ?? 0;
        }


        public async Task<AfterAuthDto> Login(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);

            if (user.Id != null && user.Password == _encrypt.HashPassword(loginDto.Password, user.Salt))
            {
                var token = _tokenService.GenerateToken(user);
                return new AfterAuthDto { Token = token, UserId = user.Id ?? 0 };
            }
            throw new AuthorizationException();
        }

        public async Task<AfterAuthDto> Register(RegDto regDto)
        {
            User user = new User() { Email = regDto.Email, Password = regDto.Password };
            using (var scope = General.Helpers.CreateTransactionScope())
            {
                await ValidateEmail(user.Email);
                int id = await CreateUser(user);
                var token = _tokenService.GenerateToken(user);
                scope.Complete();

                return new AfterAuthDto { Token = token, UserId = id };
            }
        }

        public async Task ValidateEmail(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user.Id != null) throw new DuplicateEmailException();
        }

        public Task<int> UpdateUser(int userId, string phoneNumber, string password)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteUser(int userId)
        {
            throw new NotImplementedException();
        }

    }
}
