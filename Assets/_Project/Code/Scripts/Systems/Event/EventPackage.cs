using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PackageEvent", menuName = "Scriptable Objects/Systems/Event/Package event")]
public class EventPackage : ScriptableObject
{
    private readonly HashSet<EventPackageBase> _listeners = new HashSet<EventPackageBase>();
    public virtual void Invoke(EventData eventDataPackage)
    {
        foreach (EventPackageBase eventPackageBase in _listeners)
        {
            if (eventDataPackage.HasKey(eventPackageBase.EventKey))
            {
                eventPackageBase.Invoke(eventDataPackage);
            }
        }
    }
    internal void RegisterListener(EventPackageBase listener)
    {
        _listeners.Add(listener);
    }
    internal void UnregisterListener(EventPackageBase listener)
    {
        _listeners.Remove(listener);
    }
}
