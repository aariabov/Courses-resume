using Microsoft.Extensions.Caching.Memory;

namespace Devpull;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void SetItem<TItem>(object key, TItem value)
    {
        _cache.Set(key, value);
    }

    public TItem? GetItem<TItem>(object key)
    {
        return _cache.Get<TItem>(key);
    }
}
