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
        private readonly ICodeCacheService _codeCache;
        private readonly ICodeGenerator _codeGenerator;
        private readonly INotificationQueueService _notificationQueue;

        public UserService(IUserRepository userRepository,
                           ITokenService tokenService,
                           IEncrypt encrypt,
                           ICodeCacheService codeCache,
                           ICodeGenerator codeGenerator,
                           INotificationQueueService notificationQueue)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _encrypt = encrypt;
            _codeCache = codeCache;
            _codeGenerator = codeGenerator;
            _notificationQueue = notificationQueue;
        }

        public async Task<int> CreateUser(User user)
        {
            if (user.Email == null || user.Password == null) throw new InvalidOperationException("Incorrect User Data");
            user.Salt = General.Helpers.GenerateSalt();
            user.Password = _encrypt.HashPassword(user.Password, user.Salt);
            return await _userRepository.AddAsync(user) ?? 0;
        }


        public async Task<AfterAuthDto> Login(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);

            if (user.Id != null && user.Password == _encrypt.HashPassword(loginDto.Password, user.Salt) && user.Verified)
            {
                var token = _tokenService.GenerateToken(user);
                return new AfterAuthDto { Token = token, UserId = user.Id ?? 0 };
            }
            throw new AuthorizationException();
        }

        public async Task GenerateAndSendConfiramtionCode(string? email = null, string? phone = null)
        {
            if (email == null && phone == null) throw new InvalidOperationException("Inncorrect send data");
            string code = "";

            if (email!=null)
                code = _codeGenerator.GenerateCodeForEmail(email);
            if (phone!=null)
                code = _codeGenerator.GenerateCodeForPhone(phone);

            await _codeCache.StoreCodeAsync($"email:{email}", code);

            await _notificationQueue.PublishNotification(new Domain.QueueEntities.NotificationMessage
            {
                Type = "email",
                Target = email,
                Code = code,
            });
        }

        public async Task Register(RegDto regDto)
        {
            User user = new User() { Email = regDto.Email, Password = regDto.Password };
            using (var scope = General.Helpers.CreateTransactionScope())
            {
                await ValidateEmail(user.Email);   
                int id = await CreateUser(user);
                await GenerateAndSendConfiramtionCode(regDto.Email);
                scope.Complete();
            }
        }

        public async Task<AfterAuthDto> ConfirmEmail(string email, string code)
        {
            var isValid = await _codeCache.ValidateCodeAsync($"email:{email}", code);
            if (!isValid) throw new InvalidConfirmationCodeException();

            var user = await _userRepository.GetUserByEmailAsync(email);
            user.Verified = true;
            await _userRepository.UpdateAsync(user);

            await _codeCache.RemoveCodeAsync($"email:{email}");

            var token = _tokenService.GenerateToken(user);
            int id = user.Id ?? 0;
            return new AfterAuthDto { Token = token, UserId = id };
        }

        public async Task ValidateEmail(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user.Id != null && (DateTimeOffset.UtcNow.Subtract(user.Created) <= TimeSpan.FromMinutes(10) || user.Verified)) throw new DuplicateEmailException();
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
