using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementTypeList", menuName = "Scriptable Objects/Systems/Achievements/Achievement type list")]
public class AchievementTypeList : ScriptableObject
{
    [SerializeField] private List<AchievementType> _totalAchievements;
    public List<AchievementType> AllAchievements
    {
        get
        {
            return _totalAchievements;
        }
    }
}
