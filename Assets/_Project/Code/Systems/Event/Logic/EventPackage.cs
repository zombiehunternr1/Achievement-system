using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventPackage", menuName = "Scriptable Objects/Systems/Event/Event package")]
public class EventPackage : ScriptableObject
{
    [SerializeField] private string _packageKey = System.Guid.NewGuid().ToString();
    public string PackageKey
    {
        get
        {
           return _packageKey;
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
    public virtual void InvokeBatch(List<EventData> eventDataPackages)
    {
        for (int i = 0; i < eventDataPackages.Count; i++)
        {
            EventData eventData = eventDataPackages[i];
            foreach (EventPackageHandler eventPackageHandler in _eventHandlers)
            {
                eventPackageHandler.Invoke(eventData);
            }
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
