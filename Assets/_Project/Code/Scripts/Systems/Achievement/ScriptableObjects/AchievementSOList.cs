using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementList", menuName = "Scriptable Objects/Systems/Achievements/Achievement List")]
public class AchievementSOList : ScriptableObject
{
    [SerializeField] private List<AchievementSO> _totalAchievements;
    public List<AchievementSO> AchievementList
    {
        get
        {
            return _totalAchievements;
        }
    }
}
