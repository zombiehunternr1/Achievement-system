using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEventListDoubleGenericType<T0, T1> : ScriptableObject
{
    protected List<BaseEventListDoubleGenericTypeListener<T0, T1>> _listeners = new List<BaseEventListDoubleGenericTypeListener<T0, T1>>();
    public virtual void Invoke(T0 type1, T1 type2)
    {
        foreach (BaseEventListDoubleGenericTypeListener<T0, T1> listener in _listeners)
        {
            listener.Invoke(type1, type2);
        }
    }
    public void RegisterListener(BaseEventListDoubleGenericTypeListener<T0, T1> listener)
    {
        _listeners.Add(listener);
    }
    public void UnregisterListener(BaseEventListDoubleGenericTypeListener<T0, T1> listener)
    {
        _listeners.Remove(listener);
    }
}
