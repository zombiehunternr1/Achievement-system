using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseEventListSingleGenericTypeListener<T> : MonoBehaviour
{
    [SerializeField] private List<BaseEventListSingleGenericType<T>> _baseEvents;
    [SerializeField] private UnityEvent<T> _unityEvent;
    private void OnEnable()
    {
        foreach(BaseEventListSingleGenericType<T> baseEvent in _baseEvents)
        {
            baseEvent.RegisterListener(this);
        }
    }
    private void OnDisable()
    {
        foreach (BaseEventListSingleGenericType<T> baseEvent in _baseEvents)
        {
            baseEvent.UnregisterListener(this);
        }
    }
    public void Invoke(T type)
    {
        _unityEvent.Invoke(type);
    }
}
