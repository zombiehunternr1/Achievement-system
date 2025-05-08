using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventPackage", menuName = "Scriptable Objects/Systems/Event/Event package")]
public class EventPackage : ScriptableObject
{
    private ulong _packageKey;
    public ulong PackageKey
    {
        get
        {
            return _packageKey;
        }
    }
    private readonly HashSet<EventPackageHandler> _eventHandlers = new HashSet<EventPackageHandler>();
    private void Awake()
    {
        _packageKey = (ulong)System.Guid.NewGuid().GetHashCode();
    }
    public virtual void Invoke(EventData eventDataPackage)
    {
        foreach (EventPackageHandler eventPackageHandler in _eventHandlers)
        {
            eventPackageHandler.Invoke(eventDataPackage);
        }
    }
    internal void RegisterEventHandler(EventPackageHandler eventPackageHandler)
    {
        _eventHandlers.Add(eventPackageHandler);
    }
    internal void UnregisterEventHandler(EventPackageHandler eventPackageHandler)
    {
        _eventHandlers.Remove(eventPackageHandler);
    }
}
