using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttributesData
{
    [SerializeField] private List<AchievementInfoSO> _totalAchievements;

    public List<AchievementInfoSO> TotalAchievements
    {
        get
        {
            return _totalAchievements;
        }
        set
        {
            _totalAchievements = value;
        }
    }
    public AttributesData()
    {
        _totalAchievements = TotalAchievements;
    }
}
