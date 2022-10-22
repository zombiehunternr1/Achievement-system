using UnityEngine;
using UnityEngine.Events;

public class BaseEventEmptyListener : MonoBehaviour
{
    [SerializeField] private BaseEventEmpty _baseEvent;
    [SerializeField] private UnityEvent _unityEvent;
    private void OnEnable()
    {
        _baseEvent.RegisterListener(this);
    }
    private void OnDisable()
    {
        _baseEvent.UnregisterListener(this);
    }
    public void Invoke()
    {
        _unityEvent.Invoke();
    }
}
