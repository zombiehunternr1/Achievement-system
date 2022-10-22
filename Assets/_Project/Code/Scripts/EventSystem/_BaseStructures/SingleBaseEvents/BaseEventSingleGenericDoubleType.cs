using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEventSingleGenericDoubleType<T0, T1> : ScriptableObject
{
    protected List<BaseEventSingleGenericDoubleTypeListener<T0, T1>> _listeners = new List<BaseEventSingleGenericDoubleTypeListener<T0, T1>>();
    public virtual void Invoke(T0 type1, T1 type2)
    {
        foreach (BaseEventSingleGenericDoubleTypeListener<T0, T1> listener in _listeners)
        {
            listener.Invoke(type1, type2);
        }
    }
    public void RegisterListener(BaseEventSingleGenericDoubleTypeListener<T0, T1> listener)
    {
        _listeners.Add(listener);
    }
    public void UnregisterListener(BaseEventSingleGenericDoubleTypeListener<T0, T1> listener)
    {
        _listeners.Remove(listener);
    }
}
