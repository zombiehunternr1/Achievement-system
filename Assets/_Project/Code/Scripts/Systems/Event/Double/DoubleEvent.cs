using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Double Event", menuName ="Scriptable Objects/Systems/Event/Double event")]
public class DoubleEvent : ScriptableObject
{
    private readonly HashSet<DoubleListenersList> _listeners = new HashSet<DoubleListenersList>();
    public virtual void Invoke(object objectRef1, object objectRef2)
    {
        List<DoubleEventBase> targetEvents = _listeners
            .SelectMany(listeners => listeners.BaseEvents)
            .Where(evenref => evenref.EventReferenceName == name).ToList();
        foreach (DoubleEventBase listener in targetEvents.Where(listener => listener != null))
        {
            listener.Invoke(objectRef1, objectRef2);
        }
    }
    internal void RegisterListener(DoubleListenersList listener)
    {
        _listeners.Add(listener);
    }
    internal void UnregisterListener(DoubleListenersList listener)
    {
        _listeners.Remove(listener);
    }
}
