using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SingleEventBase
{
    [SerializeField] private SingleEvent _singleEvent;
    [SerializeField] private UnityEvent<object> _unityEvent;
    public void Registering()
    {
        _singleEvent.RegisterListener(this);
    }
    public void UnRegistering()
    {
        _singleEvent.UnregisterListener(this);
    }
    internal void Invoke(object objectRef)
    {
        _unityEvent.Invoke(objectRef);
    }
}
