using UnityEngine;
using UnityEngine.Events;

public class GameEventEmptyListener : MonoBehaviour
{
    [SerializeField] private GameEventEmpty _emptyEvent;
    [SerializeField] private UnityEvent _respondse;
    private void OnEnable()
    {
        _emptyEvent.RegisterListener(this);
    }
    private void OnDisable()
    {
        _emptyEvent.UnregisterListener(this);
    }
    public void OnEventRaised()
    {
        _respondse.Invoke();
    }
}