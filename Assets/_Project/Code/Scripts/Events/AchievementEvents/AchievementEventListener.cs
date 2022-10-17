using UnityEngine;
using UnityEngine.Events;

public class AchievementEventListener : MonoBehaviour
{
    [SerializeField] private AchievementManagerSO _achievementManager;
    [SerializeField] private UnityEvent<int, int?, float?> _respondse;
    private void OnEnable()
    {
        foreach(AchievementEvent achievementEvent in _achievementManager.AchievementEventList)
        {
            achievementEvent.RegisterListener(this);
        }
    }
    private void OnDisable()
    {
        foreach (AchievementEvent achievementEvent in _achievementManager.AchievementEventList)
        {
            achievementEvent.UnregisterListener(this);
        }
    }
    public void OnEventRaised(int achievementID, int? intValue, float? floatValue)
    {
        _respondse.Invoke(achievementID, intValue, floatValue);
    }
}