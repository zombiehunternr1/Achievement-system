using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementContainer", menuName = "Scriptable Objects/Achievement container")]
public class AchievementContainerSO : ScriptableObject
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
