using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEventSingleGenericType<T> : ScriptableObject
{
    protected List<BaseEventSingleGenericTypeListener<T>> _listeners = new List<BaseEventSingleGenericTypeListener<T>>();
    public virtual void Invoke(T type)
    {
        foreach (BaseEventSingleGenericTypeListener<T> listener in _listeners)
        {
            listener.Invoke(type);
        }
    }
    public void RegisterListener(BaseEventSingleGenericTypeListener<T> listener)
    {
        _listeners.Add(listener);
    }
    public void UnregisterLister(BaseEventSingleGenericTypeListener<T> listener)
    {
        _listeners.Remove(listener);
    }
}
