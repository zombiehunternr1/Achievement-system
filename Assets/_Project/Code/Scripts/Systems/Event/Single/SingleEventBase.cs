using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SingleEventBase
{
    [SerializeField] SingleEvent _singleEvent;
    [SerializeField] private UnityEvent<object> _unityEvent;
    public string EventReferenceName
    {
        get
        {
            return _singleEvent.name;
        }
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
