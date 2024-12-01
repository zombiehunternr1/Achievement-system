using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SingleEventBase
{
    [SerializeField] private SingleEvent _singleEvent;
    [SerializeField] private UnityEvent<object> _unityEvent;
    public bool MatchesEvent(SingleEvent emptyEvent)
    {
        return _singleEvent == emptyEvent;
    }
    public void Registering(SingleListenersList listenersList)
    {
        _singleEvent.RegisterListener(listenersList);
    }
    public void UnRegistering(SingleListenersList listenersList)
    {
        _singleEvent.UnregisterListener(listenersList);
    }
    internal void Invoke(object objectRef)
    {
        _unityEvent.Invoke(objectRef);
    }
}
