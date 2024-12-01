using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Single Event", menuName = "Scriptable Objects/Systems/Event/Single event")]
public class SingleEvent : ScriptableObject
{
    private readonly HashSet<SingleEventBase> _listeners = new HashSet<SingleEventBase>();
    public virtual void Invoke(object objectRef)
    {
        foreach (SingleEventBase singleEventBase in _listeners)
        {
            singleEventBase.Invoke(objectRef);
        }
    }
    internal void RegisterListener(SingleEventBase listener)
    {
        if (!_listeners.Contains(listener))
        {
            _listeners.Add(listener);
        }
    }
    internal void UnregisterListener(SingleEventBase listener)
    {
        if (_listeners.Contains(listener))
        {
            _listeners.Remove(listener);
        }
    }
}
