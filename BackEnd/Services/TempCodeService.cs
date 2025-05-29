using Microsoft.Extensions.Caching.Memory;

namespace BackEnd.Services;

public class TempCodeService : ITempCodeService
{
    private readonly IMemoryCache _cache;

    public TempCodeService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task SaveUserInfo(string code, TempUserInfo userInfo)
    {
        _cache.Set($"temp_code_{code}", userInfo, TimeSpan.FromMinutes(5));
        return Task.CompletedTask;
    }

    public Task<TempUserInfo?> GetUserInfo(string code)
    {
        _cache.TryGetValue($"temp_code_{code}", out TempUserInfo? userInfo);
        return Task.FromResult(userInfo);
    }

    public Task RemoveCode(string code)
    {
        _cache.Remove($"temp_code_{code}");
        return Task.CompletedTask;
    }
}