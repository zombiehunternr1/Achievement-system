using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement Event Empty", menuName = "Scriptable Objects/Events/Achievement Event/Empty")]
public class AchievementEventEmpty : ScriptableObject
{
    [SerializeField] private AchievementInfo achievementToUnlock;
    private List<AchievementEventListenerEmpty> listeners = new List<AchievementEventListenerEmpty>();

    public void RaiseEmptyEvent()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(achievementToUnlock.achievementID);
        }
    }

    public void RegisterListener(AchievementEventListenerEmpty listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(AchievementEventListenerEmpty listener)
    {
        listeners.Remove(listener);
    }
}