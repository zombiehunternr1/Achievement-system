using System;
using UnityEngine;

public class EventPackageExtractor
{
    public static T ExtractData<T>(EventData eventDataPackage)
    {
        try
        {
            T value = eventDataPackage.GetData<T>(eventDataPackage.GetKey());
            if (value == null)
            {
                Debug.LogWarning("No data found for type " + typeof(T) + "in package: " + eventDataPackage.GetKey());
            }
            return value;
        }
        catch (InvalidCastException)
        {
            Debug.LogError("Failed to get data of type " + typeof(T) + " from package: " + eventDataPackage.GetKey());
            return default;
        }
    }
}