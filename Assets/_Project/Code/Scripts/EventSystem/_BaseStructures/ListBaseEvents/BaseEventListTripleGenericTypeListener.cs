using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseEventListTripleGenericTypeListener<T0, T1, T2> : MonoBehaviour
{
    [SerializeField] private List<BaseEventListTripleGenericType<T0, T1, T2>> _baseEvents;
    [SerializeField] private UnityEvent<T0, T1, T2> _unityEvent;
    private void OnEnable()
    {
        foreach (BaseEventListTripleGenericType<T0, T1, T2> baseEvent in _baseEvents)
        {
            baseEvent.RegisterListener(this);
        }
    }
    private void OnDisable()
    {
        foreach (BaseEventListTripleGenericType<T0, T1, T2> baseEvent in _baseEvents)
        {
            baseEvent.UnregisterListener(this);
        }
    }
    public void Invoke(T0 type1, T1 type2, T2 type3)
    {
        _unityEvent.Invoke(type1, type2, type3);
    }
}
