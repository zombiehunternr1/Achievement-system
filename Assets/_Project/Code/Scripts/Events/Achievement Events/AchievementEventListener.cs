using UnityEngine;
using UnityEngine.Events;

public class AchievementEventListener : MonoBehaviour
{
    [SerializeField] private AchievementEvent achievementEvent;
    [SerializeField] private UnityEvent<int, int?, float?> respondse;

    private void OnEnable()
    {
        achievementEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        achievementEvent.UnregisterListener(this);
    }

    public void OnEventRaised(int achievementID, int? intValue, float? floatValue)
    {
        respondse.Invoke(achievementID, intValue, floatValue);
    }
}