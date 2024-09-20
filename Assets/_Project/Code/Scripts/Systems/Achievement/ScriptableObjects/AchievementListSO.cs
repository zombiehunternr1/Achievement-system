using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementList", menuName = "Scriptable Objects/Achievements/Achievement List")]
public class AchievementListSO : ScriptableObject
{
    [SerializeField] private List<AchievementInfoSO> _totalAchievements;
    public List<AchievementInfoSO> AchievementList
    {
        get
        {
            return _totalAchievements;
        }
    }
}
