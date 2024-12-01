using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EmptyEventBase
{
    [SerializeField] private EmptyEvent _emptyEvent;
    [SerializeField] private UnityEvent _unityEvent;
    public bool MatchesEvent(EmptyEvent emptyEvent)
    {
        return _emptyEvent == emptyEvent;
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
