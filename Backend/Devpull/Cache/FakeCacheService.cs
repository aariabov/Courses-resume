namespace Devpull;

public class FakeCacheService : ICacheService
{
    public void SetItem<TItem>(object key, TItem value) { }

    public TItem? GetItem<TItem>(object key)
    {
        return default;
    }
}
