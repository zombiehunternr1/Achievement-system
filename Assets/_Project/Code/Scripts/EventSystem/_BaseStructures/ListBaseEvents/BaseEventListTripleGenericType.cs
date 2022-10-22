using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEventListTripleGenericType<T0, T1, T2> : ScriptableObject
{
    protected List<BaseEventListTripleGenericTypeListener<T0, T1, T2>> _listeners = new List<BaseEventListTripleGenericTypeListener<T0, T1, T2>>();
    public virtual void Invoke(T0 type1, T1 type2, T2 type3)
    {
        foreach (BaseEventListTripleGenericTypeListener<T0, T1, T2> listener in _listeners)
        {
            listener.Invoke(type1, type2, type3);
        }
    }
    public void RegisterListener(BaseEventListTripleGenericTypeListener<T0, T1, T2> listener)
    {
        _listeners.Add(listener);
    }
    public void UnregisterListener(BaseEventListTripleGenericTypeListener<T0, T1, T2> listener)
    {
        _listeners.Remove(listener);
    }
}
