using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Single Event", menuName = "Scriptable Objects/Systems/Event/Single event")]
public class SingleEvent : ScriptableObject
{
    private readonly HashSet<SingleListenersList> _listeners = new HashSet<SingleListenersList>();
    public virtual void Invoke(object objectRef)
    {
        List<SingleEventBase> targetEvents = _listeners
            .SelectMany(listeners => listeners.BaseEvents)
            .Where(eventref => eventref != null && eventref.EventReferenceName == name).ToList();
        foreach (SingleEventBase listener in targetEvents)
        {
            listener.Invoke(objectRef);
        }
    }
    internal void RegisterListener(SingleListenersList listener)
    {
        _listeners.Add(listener);

    }
    internal void UnregisterListener(SingleListenersList listener)
    {
        _listeners.Remove(listener);
    }
}
