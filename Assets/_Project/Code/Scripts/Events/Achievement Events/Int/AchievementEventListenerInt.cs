using UnityEngine;
using UnityEngine.Events;

public class AchievementEventListenerInt : MonoBehaviour
{
    [SerializeField] private AchievementEventInt intEvent;
    [SerializeField] private UnityEvent<int, int> respondse;

    private void OnEnable()
    {
        intEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        intEvent.UnregisterListener(this);
    }

    public void OnEventRaised(int achievementID, int value)
    {
        respondse.Invoke(achievementID, value);
    }
}