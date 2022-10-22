using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEventEmpty : ScriptableObject
{
    protected List<BaseEventEmptyListener> _listeners = new List<BaseEventEmptyListener>();
    public virtual void Invoke()
    {
        foreach (BaseEventEmptyListener listener in _listeners)
        {
            listener.Invoke();
        }
    }
    public void RegisterListener(BaseEventEmptyListener listener)
    {
        _listeners.Add(listener);
    }
    public void UnregisterListener(BaseEventEmptyListener listener)
    {
        _listeners.Remove(listener);
    }
}
