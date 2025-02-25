namespace Users.Application.Interfaces;

public interface ICodeGenerator
{
    string GenerateCode(int length = 6);
    string GenerateCodeForEmail(string email);
    string GenerateCodeForPhone(string phone);
}
