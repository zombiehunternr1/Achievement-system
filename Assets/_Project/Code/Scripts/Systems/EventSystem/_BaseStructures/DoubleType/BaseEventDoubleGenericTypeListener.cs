using UnityEngine;
using UnityEngine.Events;

public class BaseEventDoubleGenericTypeListener<T0, T1> : MonoBehaviour
{
    [SerializeField] private BaseEventDoubleGenericType<T0, T1> _baseEvent;
    [SerializeField] private UnityEvent<T0, T1> _unityEvent;
    private void OnEnable()
    {
        _baseEvent.RegisterListener(this);
    }
    private void OnDisable()
    {
        _baseEvent.UnregisterLister(this);
    }
    public void Invoke(T0 type0, T1 type1)
    {
        _unityEvent.Invoke(type0, type1);
    }
}
