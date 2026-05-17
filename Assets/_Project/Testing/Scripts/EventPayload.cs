using System;
using System.Collections.Generic;

public sealed class EventPayload
{
    private readonly Dictionary<Type, object> _data = new Dictionary<Type, object>();

    public ulong Key { get; private set; }
    internal bool IsInPool { get; private set; }

    internal void OnAcquire(ulong key)
    {
        Key = key;
        IsInPool = false;
    }

    internal void OnRelease()
    {
        _data.Clear();
        IsInPool = true;
    }

    public void Set<T>(T value)
    {
        _data[typeof(T)] = value;
    }

    internal void SetRaw(object value)
    {
        if (value == null)
        {
            return;
        }

        _data[value.GetType()] = value;
    }

    public T Get<T>()
    {
        if (_data.TryGetValue(typeof(T), out object value))
        {
            return (T)value;
        }

        return default;
    }

    public bool TryGet<T>(out T value)
    {
        if (_data.TryGetValue(typeof(T), out object raw))
        {
            value = (T)raw;
            return true;
        }

        value = default;
        return false;
    }

    public bool Has<T>()
    {
        return _data.ContainsKey(typeof(T));
    }
}