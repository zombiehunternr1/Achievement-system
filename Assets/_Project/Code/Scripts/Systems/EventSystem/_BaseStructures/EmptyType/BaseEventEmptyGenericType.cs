using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEventEmptyGenericType : ScriptableObject
{
    protected List<BaseEventEmptyGenericTypeListener> _listeners = new List<BaseEventEmptyGenericTypeListener>();
    public virtual void Invoke()
    {
        foreach (BaseEventEmptyGenericTypeListener listener in _listeners)
        {
            listener.Invoke();
        }
    }
    public void RegisterListener(BaseEventEmptyGenericTypeListener listener)
    {
        _listeners.Add(listener);
    }
    public void UnregisterListener(BaseEventEmptyGenericTypeListener listener)
    {
        _listeners.Remove(listener);
    }
}
