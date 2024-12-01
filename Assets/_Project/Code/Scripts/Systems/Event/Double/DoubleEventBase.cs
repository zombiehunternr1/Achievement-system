using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DoubleEventBase
{
    [SerializeField] private DoubleEvent _doubleEvent;
    [SerializeField] private UnityEvent<object, object> _unityEvent;
    public void Registering()
    {
        _doubleEvent.RegisterListener(this);
    }
    public void UnRegistering()
    {
        _doubleEvent.UnregisterListener(this);
    }
    internal void Invoke(object objectRef1, object objectRef2)
    {
        _unityEvent.Invoke(objectRef1, objectRef2);
    }
}
