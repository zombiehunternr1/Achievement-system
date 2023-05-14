using UnityEngine;
using UnityEngine.Events;

public class BaseEventTripleGenericTypeListener<T0, T1, T2> : MonoBehaviour
{
    [SerializeField] private BaseEventTripleGenericType<T0, T1, T2> _baseEvent;
    [SerializeField] private UnityEvent<T0, T1, T2> _unityEvent;
    private void OnEnable()
    {
        _baseEvent.RegisterListener(this);
    }
    private void OnDisable()
    {
        _baseEvent.UnregisterLister(this);
    }
    public void Invoke(T0 type0, T1 type1, T2 type2)
    {
        _unityEvent.Invoke(type0, type1, type2);
    }
}
