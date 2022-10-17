using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement Event", menuName = "Scriptable Objects/Events/Achievement Event")]
public class AchievementEvent : ScriptableObject
{
    [SerializeField] private AchievementInfoSO _achievementToUnlock;
    private List<AchievementEventListener> _listeners = new List<AchievementEventListener>();
    public void RaiseValueEvent(int? intValue, float? floatValue)
    {
        for (int i = _listeners.Count - 1; i >= 0; i--)
        {
            _listeners[i].OnEventRaised(_achievementToUnlock.AchievementId, intValue, floatValue);
        }
    }
    public void RegisterListener(AchievementEventListener listener)
    {
        _listeners.Add(listener);
    }
    public void UnregisterListener(AchievementEventListener listener)
    {
        _listeners.Remove(listener);
    }
}