using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementManager", menuName = "Scriptable Objects/Achievement Manager")]
public class AchievementManagerSO : ScriptableObject
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
