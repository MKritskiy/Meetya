namespace Users.Application.Interfaces
{
    public interface IEncrypt
    {
        string HashPassword(string password, string salt);
    }
}
