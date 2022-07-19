using UnityEngine;
using UnityEngine.Events;

public class GameEventEmptyListener : MonoBehaviour
{
    [SerializeField] private GameEventEmpty emptyevent;
    [SerializeField] private UnityEvent respondse;

    private void OnEnable()
    {
        emptyevent.RegisterListener(this);
    }

    private void OnDisable()
    {
        emptyevent.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        respondse.Invoke();
    }
}