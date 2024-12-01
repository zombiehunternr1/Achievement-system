using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Double Event", menuName ="Scriptable Objects/Systems/Event/Double event")]
public class DoubleEvent : ScriptableObject
{
    private readonly HashSet<DoubleEventBase> _listeners = new HashSet<DoubleEventBase>();
    public virtual void Invoke(object objectRef1, object objectRef2)
    {
        foreach (DoubleEventBase doubleEventBase in _listeners)
        {
            doubleEventBase.Invoke(objectRef1, objectRef2);
        }
    }
    internal void RegisterListener(DoubleEventBase listener)
    {
        if (!_listeners.Contains(listener))
        {
            _listeners.Add(listener);
        }
    }
    internal void UnregisterListener(DoubleEventBase listener)
    {
        if (_listeners.Contains(listener))
        {
            _listeners.Remove(listener);
        }
    }
}
