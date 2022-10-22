using UnityEngine;
using UnityEngine.Events;

public class BaseEventSingleGenericDoubleTypeListener<T0, T1> : MonoBehaviour
{
    [SerializeField] private BaseEventSingleGenericDoubleType<T0, T1> _baseEvent;
    [SerializeField] private UnityEvent<T0, T1> _unityEvent;
    private void OnEnable()
    {
        _baseEvent.RegisterListener(this);
    }
    private void OnDisable()
    {
        _baseEvent.UnregisterListener(this);
    }
    public void Invoke(T0 type1, T1 type2)
    {
        _unityEvent.Invoke(type1, type2);
    }
}
