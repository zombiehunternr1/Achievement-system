using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement Event Value", menuName = "Scriptable Objects/Events/Achievement Event/Value")]
public class AchievementEventValue : ScriptableObject
{
    [SerializeField] private AchievementInfo achievementToUnlock;
    private List<AchievementEventListenerValue> listeners = new List<AchievementEventListenerValue>();

    public void RaiseValueEvent(int? intValue, float? floatValue)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(achievementToUnlock.achievementID, intValue, floatValue);
        }
    }

    public void RegisterListener(AchievementEventListenerValue listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(AchievementEventListenerValue listener)
    {
        listeners.Remove(listener);
    }
}