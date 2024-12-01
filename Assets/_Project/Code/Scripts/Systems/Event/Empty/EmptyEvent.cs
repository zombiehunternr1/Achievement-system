using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Empty event", menuName = "Scriptable Objects/Systems/Event/Empty event")]
public class EmptyEvent : ScriptableObject
{
    private readonly HashSet<EmptyEventBase> _listeners = new HashSet<EmptyEventBase>();
    public virtual void Invoke()
    {
        foreach (EmptyEventBase emptyEventBase in _listeners)
        {
            emptyEventBase.Invoke();
        }
    }
    internal void RegisterListener(EmptyEventBase listener)
    {
        if (!_listeners.Contains(listener))
        {
            _listeners.Add(listener);
        }
    }
    internal void UnregisterListener(EmptyEventBase listener)
    {
        if (_listeners.Contains(listener))
        {
            _listeners.Remove(listener);
        }
    }
}

