using System.Collections.Generic;

public sealed class Blackboard
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