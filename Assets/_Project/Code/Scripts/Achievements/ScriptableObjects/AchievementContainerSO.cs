using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementList", menuName = "Scriptable Objects/Achievements/Achievement List")]
public class AchievementContainerSO : ScriptableObject
{
    [SerializeField] private List<AchievementInfoSO> _totalAchievements;
    public List<AchievementInfoSO> achievementList
    {
        get
        {
            return _totalAchievements;
        }
    }
}
