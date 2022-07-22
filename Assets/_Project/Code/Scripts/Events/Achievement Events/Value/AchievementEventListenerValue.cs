using UnityEngine;
using UnityEngine.Events;

public class AchievementEventListenerValue : MonoBehaviour
{
    [SerializeField] private AchievementEventValue valueEvent;
    [SerializeField] private UnityEvent<int, int?, float?> respondse;

    private void OnEnable()
    {
        valueEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        valueEvent.UnregisterListener(this);
    }

    public void OnEventRaised(int achievementID, int? intValue, float? floatValue)
    {
        respondse.Invoke(achievementID, intValue, floatValue);
    }
}