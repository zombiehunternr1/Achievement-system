using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement Event", menuName = "Scriptable Objects/Events/Achievement Event")]
public class AchievementEvent : ScriptableObject
{
    [SerializeField] private AchievementInfo achievementToUnlock;
    private List<AchievementEventListener> listeners = new List<AchievementEventListener>();

    public void RaiseValueEvent(int? intValue, float? floatValue)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(achievementToUnlock.achievementID, intValue, floatValue);
        }
    }

    public void RegisterListener(AchievementEventListener listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(AchievementEventListener listener)
    {
        listeners.Remove(listener);
    }
}