using System;
using System.Collections.Generic;
public class EventData
{
    private readonly Dictionary<ulong, Dictionary<Type, Queue<object>>> _data = new Dictionary<ulong, Dictionary<Type, Queue<object>>>();
    private ulong _packageKey;
    public Dictionary<Type, Queue<object>> GetDataForKey(ulong key)
    {
        if (_data.TryGetValue(key, out Dictionary<Type, Queue<object>> typeData))
        {
            return typeData;
        }
        return null;
    }
    public void AddData(params object[] values)
    {
        StoreDataToDictionary(_packageKey, values);
    }
    public T GetData<T>(ulong key)
    {
        if (_data.TryGetValue(key, out Dictionary<Type, Queue<object>> typeDict) && typeDict.TryGetValue(typeof(T), out Queue<object> queue) && queue.Count > 0)
        {
            return (T)queue.Dequeue();
        }
        return default;
    }
    public void Clear()
    {
        _data.Clear();
    }
    public void Reset(ulong packageKey)
    {
        Clear();
        ValidateKey(packageKey);
    }
    public ulong GetKey()
    {
        return _packageKey;
    }
    public bool HasKey(ulong key)
    {
        return _data.ContainsKey(key);
    }
    private void ValidateKey(ulong key)
    {
        _packageKey = key;
        if (!_data.ContainsKey(key))
        {
            _data[key] = new Dictionary<Type, Queue<object>>();
        }
    }
    private void StoreDataToDictionary(ulong key, object[] values)
    {
        if (!_data.TryGetValue(key, out Dictionary<Type, Queue<object>> typeDict))
        {
            typeDict = new Dictionary<Type, Queue<object>>();
            _data[key] = typeDict;
        }
        if (values == null || values.Length == 0)
        {
            if(!typeDict.TryGetValue(typeof(object), out Queue<object> queue))
            {
                queue = new Queue<object>();
                typeDict[typeof(object)] = queue;
            }
            queue.Enqueue(null);
            return;
        }
        foreach (object value in values)
        {
            Type valueType;
            if (value == null)
            {
                valueType = typeof(object);
            }
            else
            {
                valueType = value.GetType();
            }
            if (!typeDict.TryGetValue(valueType, out Queue<object> valueQueue))
            {
                valueQueue = new Queue<object>();
                typeDict[valueType] = valueQueue;
            }
            valueQueue.Enqueue(value);
        }       
    }
}