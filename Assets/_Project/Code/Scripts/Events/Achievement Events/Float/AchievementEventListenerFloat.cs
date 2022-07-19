using UnityEngine;
using UnityEngine.Events;

public class AchievementEventListenerFloat : MonoBehaviour
{
    [SerializeField] private AchievementEventFloat floatEvent;
    [SerializeField] private UnityEvent<int, float> respondse;

    private void OnEnable()
    {
        floatEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        floatEvent.UnregisterListener(this);
    }

    public void OnEventRaised(int achievementID, float value)
    {
        respondse.Invoke(achievementID, value);
    }
}