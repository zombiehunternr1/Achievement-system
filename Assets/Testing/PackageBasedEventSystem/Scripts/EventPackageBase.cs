using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class EventPackageBase
{
    [SerializeField] private EventPackage _eventPackage;
    [SerializeField] private UnityEvent<EventData> _unityEvent;
    public string EventKey
    {
        get
        {
            return _eventPackage.name;
        }
    }
    public void Registering()
    {
        _eventPackage.RegisterListener(this);
    }
    public void UnRegistering()
    {
        _eventPackage.UnregisterListener(this);
    }
    internal void Invoke(EventData eventDataPackage)
    {
        _unityEvent.Invoke(eventDataPackage);
    }
}
