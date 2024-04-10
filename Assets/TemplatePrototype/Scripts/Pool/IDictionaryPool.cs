public interface IDictionaryPool<TKey, TValue>
{
    int DictionaryCount
    {
        get;
    }
    int CountAll
    {
        get;
    }

    int Count(TKey key);
    void Clear(TKey key);
    void Clear();
    TValue Get(TKey key);
    void Release(TKey key, TValue item);
}
