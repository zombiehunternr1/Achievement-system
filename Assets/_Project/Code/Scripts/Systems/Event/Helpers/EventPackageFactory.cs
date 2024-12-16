using UnityEngine;

public class EventPackageFactory
{
    public static void BuildAndInvoke(EventPackage eventPackage, params object[] data)
    {
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
        eventPackage.Invoke(package);
    }
}
