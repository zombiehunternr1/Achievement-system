using System;
using System.Collections.Generic;
public class EventData
{
    private readonly Dictionary<string, Dictionary<Type, object>> _data = new Dictionary<string, Dictionary<Type, object>>();
    private string _packageKey;
    public EventData (string packageKey)
    {
        ValidateKey(packageKey);
    }
    public Dictionary<Type, object> GetDataForKey(string key)
    {
        if (_data.TryGetValue(key, out Dictionary<Type, object> typeObjectData))
        {
            return typeObjectData;
        }
        return new Dictionary<Type, object>();
    }
    public void AddData(params object[] values)
    {
        StoreDataToDictionary(_packageKey, values);
    }
    public T GetData<T>(string key)
    {
        //Check if the key and the type exist in the dictionary
        if (_data.TryGetValue(key, out Dictionary<Type, object> typeDict) && typeDict.TryGetValue(typeof(T), out object storedValue))
        {
            //If the stored value is a list with items, return the first item and remove it
            if (storedValue is List<object> list && list.Count > 0)
            {
                Object value = list[0];
                list.RemoveAt(0);
                return (T)value;
            }
            //If the stored value is of the expected type, return it directly
            if (storedValue is T singleValue)
            {
                return singleValue;
            }
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
            _data[key] = new Dictionary<Type, object>();
        }
    }
    private void StoreDataToDictionary(string key, object[] values)
    {
        //Handles null or empty values by storing them as an object type. This allows it for events to invoke without parameters
        if (values == null || values.Length == 0)
        {
            _data[key][typeof(object)] = null;
            return;
        }
        foreach (object value in values)
        {
            Type valueType;
            //If the value is null, store it under the generic object type as null, otherwise get the type from the value
            if (value == null)
            {
                valueType = typeof(object);
            }
            else
            {
                valueType = value.GetType();
            }
            //Check if the type isn't already a key in the inner dictionary for this key. If that is the case,
            //create a new list with the current value as its first element
            if (!_data[key].ContainsKey(valueType))
            {
                _data[key][valueType] = new List<object>()
                {
                    value
                };
            }
            //If the type exists, check if the current entry is already a list. If it is, add the current value to the list
            else if (_data[key][valueType] is List<object> list)
            {
                list.Add(value);
            }
            //If it's not a list, convert the single object to a list with the old and new values
            //This will add the existing single object along with the new value to the dictionary
            else
            {
                _data[key][valueType] = new List<object>
                {
                    _data[key][valueType],
                    value
                };
            }
        }
    }
}