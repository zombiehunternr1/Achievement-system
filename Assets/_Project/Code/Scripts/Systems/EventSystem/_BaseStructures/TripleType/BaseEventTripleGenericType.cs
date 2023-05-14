using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEventTripleGenericType<T0, T1, T2> : ScriptableObject
{
    protected List<BaseEventTripleGenericTypeListener<T0, T1, T2>> _listeners = new List<BaseEventTripleGenericTypeListener<T0, T1, T2>>();
    public virtual void Invoke(T0 type0, T1 type1, T2 type2)
    {
        foreach (BaseEventTripleGenericTypeListener<T0, T1, T2> listener in _listeners)
        {
            listener.Invoke(type0, type1, type2);
        }
    }
    public void RegisterListener(BaseEventTripleGenericTypeListener<T0, T1, T2> listener)
    {
        _listeners.Add(listener);
    }
    public void UnregisterLister(BaseEventTripleGenericTypeListener<T0, T1, T2> listener)
    {
        _listeners.Remove(listener);
    }
}
