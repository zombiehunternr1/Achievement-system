using UnityEngine;
using System;

[Serializable]
public class AchievementData
{
    [SerializeField] private AchievementSOList _achievementListReference;
    [SerializeField] private bool _hasCustomGoalAmount;
    [SerializeField] private int _customGoalAmount;
    public (int currentAmount, int totalAmount) GetProgressionDisplay()
    {
        int currentAmount = 0;
        int goalAmount;
        if (_hasCustomGoalAmount)
        {
            goalAmount = _customGoalAmount;
        }
        else
        {
            goalAmount = GetActualAchievementCount;
        }
        foreach (AchievementSO achievement in _achievementListReference.AchievementList)
        {
            if (achievement.IsUnlocked && achievement.CompletionEnumRequirement != CompletionEnumRequirement.AchievementRequirement)
            {
                currentAmount++;
                if (currentAmount >= goalAmount)
                {
                    break;
                }
            }
        }
        return (currentAmount, goalAmount);
    }
    public bool IsRequirementMet()
    {
        int currentAmount = 0;
        int goalAmount;
        if (_hasCustomGoalAmount)
        {
            goalAmount = _customGoalAmount;
        }
        else
        {
            goalAmount = GetActualAchievementCount;
        }
        for (int i = 0; i < _achievementListReference.AchievementList.Count; i++)
        {
            AchievementSO achievement = _achievementListReference.AchievementList[i];
            if (achievement.CompletionEnumRequirement == CompletionEnumRequirement.AchievementRequirement)
            {
                continue;
            }
            if (!achievement.IsUnlocked)
            {
                continue;
            }
            currentAmount++;
            if (currentAmount >= goalAmount)
            {
                return true;
            }
        }
        return false;
    }
    private int GetActualAchievementCount
    {
        get
        {
            int amount = 0;
            for (int i = 0; i < _achievementListReference.AchievementList.Count; i++)
            {
                AchievementSO achievement = _achievementListReference.AchievementList[i];
                if (achievement != null && achievement.CompletionEnumRequirement != CompletionEnumRequirement.AchievementRequirement)
                {
                    amount++;
                }
            }
            return amount;
        }
    }
}
