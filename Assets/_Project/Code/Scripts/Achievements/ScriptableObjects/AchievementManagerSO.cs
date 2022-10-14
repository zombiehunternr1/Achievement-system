using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementManager", menuName = "Scriptable Objects/Achievement Manager")]
public class AchievementManagerSO : ScriptableObject
{
    [SerializeField] private List<AchievementInfoSO> _totalAchievements;
    [SerializeField] private List<AchievementEvent> _achievementEvents;
    public List<AchievementInfoSO> AchievementList
    {
        get
        {
            return _totalAchievements;
        }
    }
    public List<AchievementEvent> AchievementEventList
    {
        get
        {
            return _achievementEvents;
        }
    }
}
