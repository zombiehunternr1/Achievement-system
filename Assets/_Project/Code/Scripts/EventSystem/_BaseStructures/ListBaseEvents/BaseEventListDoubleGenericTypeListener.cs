using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseEventListDoubleGenericTypeListener<T0, T1> : MonoBehaviour
{
    [SerializeField] private List<BaseEventListDoubleGenericType<T0, T1>> _baseEvents;
    [SerializeField] private UnityEvent<T0, T1> _unityEvent;
    private void OnEnable()
    {
        foreach (BaseEventListDoubleGenericType<T0, T1> baseEvent in _baseEvents)
        {
            baseEvent.RegisterListener(this);
        }
    }
    private void OnDisable()
    {
        foreach (BaseEventListDoubleGenericType<T0, T1> baseEvent in _baseEvents)
        {
            baseEvent.UnregisterListener(this);
        }
    }
    public void Invoke(T0 type1, T1 type2)
    {
        _unityEvent.Invoke(type1, type2);
    }
}
