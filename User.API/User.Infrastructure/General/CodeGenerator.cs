using System.Security.Cryptography;
using Users.Application.Interfaces;

namespace Users.Infrastructure.General;

public class CodeGenerator : ICodeGenerator
{
    public string GenerateCode(int length = 6)
    {
        if (length < 1 || length > 9)
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be between 1 and 9.");
        using var rng = RandomNumberGenerator.Create();
        var byteArray = new byte[4];
        rng.GetBytes(byteArray);
        var randNum = BitConverter.ToUInt32(byteArray, 0) % (uint)Math.Pow(10,length);
        return randNum.ToString($"D{length}");
    }

    public string GenerateCodeForEmail(string email) => GenerateCode();

    public string GenerateCodeForPhone(string phone) => GenerateCode();
}
