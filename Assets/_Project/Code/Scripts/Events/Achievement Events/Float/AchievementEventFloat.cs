using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement Event Float", menuName = "Scriptable Objects/Events/Achievement Event/Float")]
public class AchievementEventFloat : ScriptableObject
{
    [SerializeField] private AchievementInfo achievementToUnlock;
    private List<AchievementEventListenerFloat> listeners = new List<AchievementEventListenerFloat>();

    public void RaiseFloatEvent(float value)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(achievementToUnlock.achievementID, value);
        }
    }

    public void RegisterListener(AchievementEventListenerFloat listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(AchievementEventListenerFloat listener)
    {
        listeners.Remove(listener);
    }
}