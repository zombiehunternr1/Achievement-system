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
        if (string.IsNullOrEmpty(eventPackage.name))
        {
            Debug.LogError("Package key cannot be null or empty!");
            return;
        }
        EventData package = new EventData(eventPackage.name);
        if (data != null && data.Length > 0)
        {
            package.AddData(data);
        }
        else
        {
            Debug.LogWarning("No data provided for package " +  eventPackage.name + ".");
        }
        eventPackage.Invoke(package);
    }
}
