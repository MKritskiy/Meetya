using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Users.Application.Interfaces;

namespace Users.Infrastructure.General
{
    public class Encrypt : IEncrypt
    {
        public string HashPassword(string password, string salt) 
            => Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password,
                    System.Text.Encoding.ASCII.GetBytes(salt),
                    KeyDerivationPrf.HMACSHA512,
                    5000,
                    64
                )
            );
    }
}
