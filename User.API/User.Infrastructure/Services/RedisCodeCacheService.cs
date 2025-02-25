using StackExchange.Redis;
using Users.Application.Interfaces;

namespace Users.Infrastructure.Services;

public class RedisCodeCacheService : ICodeCacheService
{
    private readonly IDatabase _redisDb;
    private readonly string firstKeyPart = "confirmation:";
    public RedisCodeCacheService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task RemoveCodeAsync(string key)
    {
       await _redisDb.KeyDeleteAsync($"{firstKeyPart}{key}");
    }

    public async Task<string?> RetrieveCodeAsync(string key)
    {
        return await _redisDb.StringGetAsync($"{firstKeyPart}{key}");
    }

    public async Task StoreCodeAsync(string key, string code, TimeSpan? expiry = null)
    {
        await _redisDb.StringSetAsync($"{firstKeyPart}{key}", code, expiry ?? TimeSpan.FromMinutes(10));
    }

    public async Task<bool> ValidateCodeAsync(string key, string code)
    {
        var storedCode = await RetrieveCodeAsync(key);
        return storedCode == code;
    }
}
