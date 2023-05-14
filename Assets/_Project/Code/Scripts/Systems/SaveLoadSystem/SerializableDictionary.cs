using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> _keys = new List<TKey>();
    [SerializeField] private List<TValue> _values = new List<TValue>();
    public void OnAfterDeserialize()
    {
        Clear();
        if(Keys.Count != Values.Count)
        {
            Debug.LogError("Tried to deserialize a Serializable Dictionary, but the amount of keys (" + Keys.Count + ") does not match the number of values (" + Values.Count + ")");
            return;
        }
        for(int i = 0; i < _keys.Count; i++)
        {
            Add(_keys[i], _values[i]);
        }
    }
    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();
        foreach(KeyValuePair<TKey, TValue> pair in this)
        {
            _keys.Add(pair.Key);
            _values.Add(pair.Value);
        }
    }
}
