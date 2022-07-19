
using UnityEngine;
using UnityEngine.Events;

public class GameEventIntListener : MonoBehaviour
{
    [SerializeField] private GameEventInt intevent;
    [SerializeField] private UnityEvent<int> respondse;

    private void OnEnable()
    {
        intevent.RegisterListener(this);
    }

    private void OnDisable()
    {
        intevent.UnregisterListener(this);
    }

    public void OnEventRaised(int value)
    {
        respondse.Invoke(value);
    }
}