using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventPackage", menuName = "Scriptable Objects/Systems/Event/Event package")]
public class EventPackage : ScriptableObject
{
    private readonly HashSet<EventPackageHandler> _listeners = new HashSet<EventPackageHandler>();
    public virtual void Invoke(EventData eventDataPackage)
    {
        foreach (EventPackageHandler eventPackageBase in _listeners)
        {
            eventPackageBase.Invoke(eventDataPackage);
        }
    }
    internal void RegisterListener(EventPackageHandler listener)
    {
        _listeners.Add(listener);
    }
    internal void UnregisterListener(EventPackageHandler listener)
    {
        _listeners.Remove(listener);
    }
}
