using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement Event Int", menuName = "Scriptable Objects/Events/Achievement Event/Int")]
public class AchievementEventInt : ScriptableObject
{
    [SerializeField] private AchievementInfo achievementToUnlock;
    private List<AchievementEventListenerInt> listeners = new List<AchievementEventListenerInt>();

    public void RaiseIntEvent(int value)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(achievementToUnlock.achievementID, value);
        }
    }

    public void RegisterListener(AchievementEventListenerInt listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(AchievementEventListenerInt listener)
    {
        listeners.Remove(listener);
    }
}