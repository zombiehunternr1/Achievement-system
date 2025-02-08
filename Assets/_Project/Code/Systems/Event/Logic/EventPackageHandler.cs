using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class EventPackageHandler
{
    [SerializeField] private EventPackage _eventPackage;
    [SerializeField] private UnityEvent<EventData> _unityEvent;
    public void Register()
    {
        if (_eventPackage == null)
        {
            Debug.LogError("EventPackage reference is missing!");
            return;
        }
        _eventPackage.RegisterEventHandler(this);
    }
    public void Unregister()
    {
        if (_eventPackage != null)
        {
            _eventPackage.UnregisterEventHandler(this);
        }
    }
    internal void Invoke(EventData eventDataPackage)
    {
        _unityEvent.Invoke(eventDataPackage);
    }
}
