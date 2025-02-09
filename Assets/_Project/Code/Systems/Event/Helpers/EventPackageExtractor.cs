using System;
using System.Collections.Generic;
using UnityEngine;

public class EventPackageExtractor
{
    public static bool ContainsData(EventData eventData)
    {
        return eventData != null && eventData.GetKey() > 0;
    }
    public static object ExtractAdditionalData(EventData eventData)
    {
        try
        {
            ulong key = eventData.GetKey();
            if (!eventData.HasKey(key) || eventData.GetDataForKey(key).Count == 0)
            {
                return null;
            }
            foreach (KeyValuePair<Type, Queue<object>> entry in eventData.GetDataForKey(key))
            {
                if (entry.Key == typeof(string))
                {
                    continue;
                }
                if (entry.Value is Queue<object> queue && queue.Count > 0)
                {
                    return queue.Peek();
                }
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to extract second data: " + e.Message);
            return null;
        }
    }
    public static T ExtractEventData<T>(EventData eventData)
    {
        try
        {
            T value = eventData.GetData<T>(eventData.GetKey());
            if (value == null)
            {
                Debug.LogWarning("No data found for type " + typeof(T) + " in package: " + eventData.GetKey());
            }
            return value;
        }
        catch (InvalidCastException)
        {
            Debug.LogError("Failed to get data of type " + typeof(T) + " from package: " + eventData.GetKey());
            return default;
        }
    }
}
