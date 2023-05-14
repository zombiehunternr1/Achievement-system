using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEventDoubleGenericType<T0, T1> : ScriptableObject
{
    protected List<BaseEventDoubleGenericTypeListener<T0, T1>> _listeners = new List<BaseEventDoubleGenericTypeListener<T0, T1>>();
    public virtual void Invoke(T0 type0, T1 type1)
    {
        foreach (BaseEventDoubleGenericTypeListener<T0, T1> listener in _listeners)
        {
            listener.Invoke(type0, type1);
        }
    }
    public void RegisterListener(BaseEventDoubleGenericTypeListener<T0, T1> listener)
    {
        _listeners.Add(listener);
    }
    public void UnregisterLister(BaseEventDoubleGenericTypeListener<T0, T1> listener)
    {
        _listeners.Remove(listener);
    }
}
