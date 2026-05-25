using UnityEngine;
using System;

[Serializable]
public class AchievementData
{
    [SerializeField] private AchievementTypeList _achievementListReference;
    [SerializeField] private bool _hasCustomGoalAmount;
    [SerializeField] private int _customGoalAmount;

    public (int currentAmount, int totalAmount) GetProgressionDisplay(Func<string, bool> isUnlocked)
    {
        int currentAmount = 0;
        int goalAmount = GetGoalAmount();

        foreach (AchievementType achievement in _achievementListReference.AllAchievements)
        {
            if (IsEligibleForProgress(achievement) && isUnlocked(achievement.AchievementId))
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

    public bool IsRequirementMet(Func<string, bool> isUnlocked)
    {
        int currentAmount = 0;
        int goalAmount = GetGoalAmount();

        foreach (AchievementType achievement in _achievementListReference.AllAchievements)
        {
            if (IsEligibleForProgress(achievement) && isUnlocked(achievement.AchievementId))
            {
                currentAmount++;

                if (currentAmount >= goalAmount)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private int GetGoalAmount()
    {
        if (_hasCustomGoalAmount)
        {
            return _customGoalAmount;
        }

        return GetActualAchievementCount();
    }

    private int GetActualAchievementCount()
    {
        int amount = 0;

        foreach (AchievementType achievement in _achievementListReference.AllAchievements)
        {
            if (IsEligibleForProgress(achievement))
            {
                amount++;
            }
        }

        return amount;
    }

    private bool IsEligibleForProgress(AchievementType achievement)
    {
        return achievement != null &&
               achievement.CompletionEnumRequirement != CompletionRequirementType.AchievementRequirement;
    }
}