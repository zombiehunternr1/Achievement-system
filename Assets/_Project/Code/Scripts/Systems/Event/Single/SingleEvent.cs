using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Single Event", menuName = "Scriptable Objects/Systems/Event/Single event")]
public class SingleEvent : ScriptableObject
{
    private readonly HashSet<SingleListenersList> _listeners = new HashSet<SingleListenersList>();
    public virtual void Invoke(object objectRef)
    {
        foreach (SingleListenersList listener in _listeners)
        {
            foreach (SingleEventBase baseEvent in listener.BaseEvents)
            {
                if (baseEvent != null && baseEvent.EventReferenceName == name)
                {
                    baseEvent.Invoke(objectRef);
                }
            }
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
