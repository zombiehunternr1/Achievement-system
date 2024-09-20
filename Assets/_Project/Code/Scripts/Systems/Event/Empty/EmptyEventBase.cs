using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EmptyEventBase
{
    [SerializeField] EmptyEvent _emptyEvent;
    [SerializeField] private UnityEvent _unityEvent;
    public string EventReferenceName
    {
        get
        {
            return _emptyEvent.name;
        }
    }
    public void Registering(EmptyListenersList listenersList)
    {
        _emptyEvent.RegisterListener(listenersList);
    }
    public void UnRegistering(EmptyListenersList listenersList)
    {
        _emptyEvent.UnregisterListener(listenersList);
    }
    internal void Invoke()
    {
        _unityEvent.Invoke();
    }
}
