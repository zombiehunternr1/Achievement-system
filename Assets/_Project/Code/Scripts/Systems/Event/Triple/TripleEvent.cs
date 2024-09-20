using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Triple Event", menuName = "Scriptable Objects/Systems/Event/Triple event")]
public class TripleEvent : ScriptableObject
{
    private readonly HashSet<TripleListenersList> _listeners = new HashSet<TripleListenersList>();
    public virtual void Invoke(object objectRef1, object objectRef2, object objectRef3)
    {
        List<TripleEventBase> targetEvents = _listeners
            .SelectMany(listeners => listeners.BaseEvents)
            .Where(evenref => evenref.EventReferenceName == name).ToList();
        foreach (TripleEventBase listener in targetEvents.Where(listener => listener != null))
        {
            listener.Invoke(objectRef1, objectRef2, objectRef3);
        }
    }
    internal void RegisterListener(TripleListenersList listener)
    {
        _listeners.Add(listener);
    }
    internal void UnregisterListener(TripleListenersList listener)
    {
        _listeners.Remove(listener);
    }
}
