using UnityEngine;
using System;

[Serializable]
public class AchievementData
{
    [SerializeField] private AchievementSOList _achievementListReference;
    [SerializeField] private bool _customGoalAmount;
    [SerializeField] private int _goalAchievementAmount;
    public AchievementSOList AchievementListReference
    {
        get
        {
            return _achievementListReference;
        }
    }
    public bool CustomGoalAmount
    {
        get
        {
            return _customGoalAmount;
        }
    }
    public int GoalAchievementAmount
    {
        get
        {
            return _goalAchievementAmount;
        }
    }
}
