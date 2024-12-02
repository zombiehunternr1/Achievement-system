using UnityEngine;
using System;

[Serializable]
public class AchievementData
{
    [SerializeField] private AchievementSOList _achievementListReference;
    [SerializeField] private bool _hasCustomGoalAmount;
    [SerializeField] private int _goalAmount;
    public AchievementSOList AchievementListReference
    {
        get
        {
            return _achievementListReference;
        }
    }
    public bool HasCustomGoalAmount
    {
        get
        {
            return _hasCustomGoalAmount;
        }
    }
    public int GoalAmount
    {
        get
        {
            return _goalAmount;
        }
    }
}
