using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Empty event", menuName = "Scriptable Objects/Systems/Event/Empty event")]
public class EmptyEvent : ScriptableObject
{
    private readonly List<EmptyEventBase> _listeners = new List<EmptyEventBase>();
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

