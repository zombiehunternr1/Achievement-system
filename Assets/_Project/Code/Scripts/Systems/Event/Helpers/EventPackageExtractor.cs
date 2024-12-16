using System;
using System.Collections.Generic;
using UnityEngine;

public class EventPackageExtractor
{
    public static bool ContainsData(EventData eventData)
    {
        if (eventData == null || eventData.GetKey().Length <= 1)
        {
            return false;
        }
        return true;
    }
    public static object ExtractAdditionalData(EventData eventData)
    {
        try
        {
            string key = eventData.GetKey();
            if (!eventData.HasKey(key))
            {
                return null;
            }
            Dictionary<Type, object> innerDict = eventData.GetDataForKey(key);
            if (innerDict.Count <= 1)
            {
                return null;
            }
            foreach (KeyValuePair<Type, object> entry in innerDict)
            {
                if (entry.Key == typeof(string))
                {
                    continue;
                }
                if (entry.Value is List<object> list && list.Count > 0)
                {
                    return list[0];
                }
                return entry.Value;
            }
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to extract second data: " + ex.Message);
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