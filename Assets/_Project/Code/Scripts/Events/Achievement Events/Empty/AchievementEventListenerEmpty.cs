using UnityEngine;
using UnityEngine.Events;

public class AchievementEventListenerEmpty : MonoBehaviour
{
    [SerializeField] private AchievementEventEmpty emptyEvent;
    [SerializeField] private UnityEvent<int> respondse;

    private void OnEnable()
    {
        emptyEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        emptyEvent.UnregisterListener(this);
    }

    public void OnEventRaised(int value)
    {
        respondse.Invoke(value);
    }
}