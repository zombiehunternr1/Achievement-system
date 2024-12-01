using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EmptyEventBase
{
    [SerializeField] private EmptyEvent _emptyEvent;
    [SerializeField] private UnityEvent _unityEvent;
    public void Registering()
    {
        _emptyEvent.RegisterListener(this);
    }
    public void UnRegistering()
    {
        _emptyEvent.UnregisterListener(this);
    }
    internal void Invoke()
    {
        _unityEvent.Invoke();
    }
}
