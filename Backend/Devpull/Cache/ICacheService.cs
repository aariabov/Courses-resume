namespace Devpull;

public interface ICacheService
{
    void SetItem<TItem>(object key, TItem value);
    TItem? GetItem<TItem>(object key);
}
