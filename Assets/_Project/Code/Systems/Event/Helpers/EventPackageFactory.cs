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
        if (eventPackage.PackageKey == 0)
        {
            Debug.LogError("Package key cannot be null or empty!");
            return;
        }
        EventDataPool.GetEventData(eventPackage.PackageKey, out EventData package);
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
}
