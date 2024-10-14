using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Double Event", menuName ="Scriptable Objects/Systems/Event/Double event")]
public class DoubleEvent : ScriptableObject
{
    private readonly HashSet<DoubleListenersList> _listeners = new HashSet<DoubleListenersList>();
    public virtual void Invoke(object objectRef1, object objectRef2)
    {
        foreach (DoubleListenersList listener in _listeners)
        {
            foreach (DoubleEventBase baseEvent in listener.BaseEvents)
            {
                if (baseEvent != null && baseEvent.EventReferenceName == name)
                {
                    baseEvent.Invoke(objectRef1, objectRef2);
                }
            }
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
