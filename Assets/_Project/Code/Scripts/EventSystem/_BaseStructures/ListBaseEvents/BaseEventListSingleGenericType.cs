using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEventListSingleGenericType<T> : ScriptableObject
{
    protected List<BaseEventListSingleGenericTypeListener<T>> _listeners = new List<BaseEventListSingleGenericTypeListener<T>>();
    public virtual void Invoke(T type)
    {
        foreach (BaseEventListSingleGenericTypeListener<T> listener in _listeners)
        {
            listener.Invoke(type);
        }
    }
    public void RegisterListener(BaseEventListSingleGenericTypeListener<T> listener)
    {
        _listeners.Add(listener);
    }
    public void UnregisterListener(BaseEventListSingleGenericTypeListener<T> listener)
    {
        _listeners.Remove(listener);
    }
}
