using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Empty event", menuName = "Scriptable Objects/Systems/Event/Empty event")]
public class EmptyEvent : ScriptableObject
{
    private readonly List<EmptyListenersList> _listeners = new List<EmptyListenersList>();
    public virtual void Invoke()
    {
        foreach (EmptyListenersList listener in _listeners)
        {
            foreach (EmptyEventBase baseEvent in listener.BaseEvents)
            {
                if (baseEvent != null && baseEvent.MatchesEvent(this))
                {
                    baseEvent.Invoke();
                }
            }
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
