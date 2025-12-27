using System.Collections.Generic;

public interface IBlackboard
{
    bool TryGet<T>(string key, out T value);
    void Set<T>(string key, T value);
}

public sealed class DictionaryBlackboard : IBlackboard
{
    private readonly Dictionary<string, object> _data = new();

    public bool TryGet<T>(string key, out T value)
    {
        if (_data.TryGetValue(key, out var obj) && obj is T t)
        {
            value = t;
            return true;
        }
        value = default!;
        return false;
    }

    public void Set<T>(string key, T value) => _data[key] = value!;
}