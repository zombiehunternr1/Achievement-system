using UnityEngine;
using UnityEngine.Events;

public class BaseEventSingleGenericTripleTypeListener<T0, T1, T2> : MonoBehaviour
{
    [SerializeField] private BaseEventSingleGenericTripleType<T0, T1, T2> _baseEvent;
    [SerializeField] private UnityEvent<T0, T1, T2> _unityEvent;
    private void OnEnable()
    {
        _baseEvent.RegisterListener(this);
    }
    private void OnDisable()
    {
        _baseEvent.UnregisterListener(this);
    }
    public void Invoke(T0 type1, T1 type2, T2 type3)
    {
        _unityEvent.Invoke(type1, type2, type3);
    }
}
