using System;
using System.Collections.Generic;
public class EventData
{
    private readonly Dictionary<string, Dictionary<Type, Queue<object>>> _data = new Dictionary<string, Dictionary<Type, Queue<object>>>();
    private string _packageKey;
    public EventData (string packageKey)
    {
        ValidateKey(packageKey);
    }
    public Dictionary<Type, Queue<object>> GetDataForKey(string key)
    {
        if (_data.TryGetValue(key, out Dictionary<Type, Queue<object>> typeData))
        {
            return typeData;
        }
        return new Dictionary<Type, Queue<object>>();
    }
    public void AddData(params object[] values)
    {
        StoreDataToDictionary(_packageKey, values);
    }
    public T GetData<T>(string key)
    {
        if (_data.TryGetValue(key, out Dictionary<Type, Queue<object>> typeDict) && typeDict.TryGetValue(typeof(T), out Queue<object> queue) && queue.Count > 0)
        {
            return (T)queue.Dequeue();
        }
        return default;
    }
    public string GetKey()
    {
        return _packageKey;
    }
    public bool HasKey(string key)
    {
        return _data.ContainsKey(key);
    }
    private void ValidateKey(string key)
    {
        _packageKey = key;
        if (!_data.ContainsKey(key))
        {
            _data[key] = new Dictionary<Type, Queue<object>>();
        }
    }
    private void StoreDataToDictionary(string key, object[] values)
    {
        if (values == null || values.Length == 0)
        {
            if (!_data[key].ContainsKey(typeof(object)))
            {
                _data[key][typeof(object)] = new Queue<object>();
            }
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
            if (!_data[key].ContainsKey(valueType))
            {
                _data[key][valueType] = new Queue<object>();
            }
            _data[key][valueType].Enqueue(value);
        }       
    }
}