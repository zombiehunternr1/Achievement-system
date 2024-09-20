using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName ="Empty event", menuName = "Scriptable Objects/Systems/Event/Empty event")]
public class EmptyEvent : ScriptableObject
{
    private readonly HashSet<EmptyListenersList> _listeners = new HashSet<EmptyListenersList>();
    public virtual void Invoke()
    {
        List<EmptyEventBase> targetEvents = _listeners
            .SelectMany(listeners => listeners.BaseEvents)
            .Where(evenref => evenref.EventReferenceName == name).ToList();
        foreach (EmptyEventBase listener in targetEvents.Where(listener => listener != null))
        {
            listener.Invoke();
        }
    }
    internal void RegisterListener(EmptyListenersList listener)
    {
        _listeners.Add(listener);
    }
    internal void UnregisterListener(EmptyListenersList listener)
    {
        _listeners.Remove(listener);
    }
}
