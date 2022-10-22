using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEventSingleGenericSingleType<T> : ScriptableObject
{
    protected List<BaseEventSingleGenericSingleTypeListener<T>> _listeners = new List<BaseEventSingleGenericSingleTypeListener<T>>();
    public virtual void Invoke(T type)
    {
        foreach(BaseEventSingleGenericSingleTypeListener<T> listener in _listeners)
        {
            listener.Invoke(type);
        }
    }
    public void RegisterListener(BaseEventSingleGenericSingleTypeListener<T> listener)
    {
        _listeners.Add(listener);
    }
    public void UnregisterListener(BaseEventSingleGenericSingleTypeListener<T> listener)
    {
        _listeners.Remove(listener);
    }
}
