using System.Collections.Generic;
using UnityEngine;

public class EventPackageFactory
{
    public static void BuildAndInvoke(EventPackage eventPackage, params object[] data)
    {
        if (eventPackage == null)
        {
            Debug.LogError("EventPackage cannot be null!");
            return;
        }
        if (string.IsNullOrEmpty(eventPackage.PackageKey))
        {
            Debug.LogError("Package key cannot be null or empty!");
            return;
        }
        EventData package = EventDataPool.GetEventData(eventPackage.PackageKey);
        try
        {
            if (data != null && data.Length > 0)
            {
                package.AddData(data);
            }
            eventPackage.Invoke(package);
        }
        finally
        {
            EventDataPool.Release(package);
        }
    }
    public static void BuildAndInvokeBatch(EventPackage eventPackage, List<object[]> dataList)
    {
        if (eventPackage == null)
        {
            Debug.LogError("EventPackage cannot be null!");
            return;
        }
        if (string.IsNullOrEmpty(eventPackage.PackageKey))
        {
            Debug.LogError("Package key cannot be null or empty!");
            return;
        }
        List<EventData> dataBatch = new List<EventData>(dataList.Count);
        try
        {
            foreach (object[] data in dataList)
            {
                EventData package = EventDataPool.GetEventData(eventPackage.PackageKey);
                package.AddData(data);
                dataBatch.Add(package);
            }
            eventPackage.InvokeBatch(dataBatch);
        }
        finally
        {
            foreach (EventData package in dataBatch)
            {
                EventDataPool.Release(package);
            }
        }
    }
}
