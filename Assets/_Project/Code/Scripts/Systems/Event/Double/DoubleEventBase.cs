using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DoubleEventBase
{
    [SerializeField] private DoubleEvent _doubleEvent;
    [SerializeField] private UnityEvent<object, object> _unityEvent;
    public bool MatchesEvent(DoubleEvent emptyEvent)
    {
        return _doubleEvent == emptyEvent;
    }
    public void Registering(DoubleListenersList listenersList)
    {
        _doubleEvent.RegisterListener(listenersList);
    }
    public void UnRegistering(DoubleListenersList listenersList)
    {
        _doubleEvent.UnregisterListener(listenersList);
    }
    internal void Invoke(object objectRef1, object objectRef2)
    {
        _unityEvent.Invoke(objectRef1, objectRef2);
    }
}
