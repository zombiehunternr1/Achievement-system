using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TripleEventBase
{
    [SerializeField] TripleEvent _tripleEvent;
    [SerializeField] private UnityEvent<object, object, object> _unityEvent;
    public string EventReferenceName
    {
        get
        {
            return _tripleEvent.name;
        }
    }
    public void Registering(TripleListenersList listenersList)
    {
        _tripleEvent.RegisterListener(listenersList);
    }
    public void UnRegistering(TripleListenersList listenersList)
    {
        _tripleEvent.UnregisterListener(listenersList);
    }
    internal void Invoke(object objectRef1, object objectRef2, object objectRef3)
    {
        _unityEvent.Invoke(objectRef1, objectRef2, objectRef3);
    }
}
