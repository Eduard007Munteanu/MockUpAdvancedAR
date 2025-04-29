using System.Collections.Generic;
using System;

public class DataPacket
{
    private readonly Dictionary<string, object> data = new Dictionary<string, object>();

    public DataPacket(params (string key, object value)[] entries)
    {
        foreach (var (key, value) in entries)
        {
            data[key] = value;
        }
    }

    public void Add(string key, object value)
    {
        data[key] = value;
    }

    public T Get<T>(string key)
    {
        if (!data.ContainsKey(key))
            throw new KeyNotFoundException($"Key '{key}' not found in DataPacket.");

        return (T)data[key];
    }

    public bool TryGet<T>(string key, out T value)
    {
        value = default;
        if (data.TryGetValue(key, out var obj) && obj is T typedObj)
        {
            value = typedObj;
            return true;
        }
        return false;
    }
}
