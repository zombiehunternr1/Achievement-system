using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventPackage", menuName = "Scriptable Objects/Systems/Event/Event package")]
public class EventPackage : ScriptableObject
{
    private static ulong _packageCounter = 1;
    private ulong _packageKey = 0; 
    public ulong PackageKey
    {
        get
        {
            return _packageKey;
        }
    }
    private void Awake()
    {
        if (_packageKey == 0)
        {
            _packageKey = _packageCounter++;
        }
    }
    private readonly HashSet<EventPackageHandler> _eventHandlers = new HashSet<EventPackageHandler>();
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
