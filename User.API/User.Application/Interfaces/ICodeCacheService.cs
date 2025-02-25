namespace Users.Application.Interfaces;

public interface ICodeCacheService
{
    Task StoreCodeAsync(string key, string code, TimeSpan? expiry = null);
    Task<string?> RetrieveCodeAsync(string key);
    Task<bool> ValidateCodeAsync(string key, string code);
    Task RemoveCodeAsync(string v);
}
