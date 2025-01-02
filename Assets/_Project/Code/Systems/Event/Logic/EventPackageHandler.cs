using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class EventPackageHandler
{
    [SerializeField] private EventPackage _eventPackage;
    [SerializeField] private UnityEvent<EventData> _unityEvent;
    public void Registering()
    {
        _eventPackage.RegisterEventHandler(this);
    }
    public void UnRegistering()
    {
        _eventPackage.UnregisterEventHandler(this);
    }
    internal void Invoke(EventData eventDataPackage)
    {
        _unityEvent.Invoke(eventDataPackage);
    }
}
