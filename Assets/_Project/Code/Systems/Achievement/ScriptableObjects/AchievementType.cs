using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementType", menuName = "Scriptable Objects/Systems/Achievements/Achievement type")]
public class AchievementType : AchievementBase
{
    [SerializeField] private RewardTier _rewardTier;
    [SerializeField] private CompletionRequirementType _completionRequirement;
    [SerializeField] private ProgressionData _progressionData;
    [SerializeField] private AchievementData _achievementData;
    [SerializeField] private ValueData _valueData;
    [SerializeField] private bool _isUnlockedAfterAchievement;
    [SerializeField] private List<AchievementType> _unlockAfterAchievements;

    // -----------------------------------------------------------------------
    // COLLECTABLE EXTENSION
    // This block is the only collectable reference in the core achievement system.
    // To use this system in a project without collectables:
    //   - Remove this block (the field and the property below)
    //   - Remove CollectableData.cs and CollectableAchievementBridge.cs
    //   - Everything else works without modification
    // -----------------------------------------------------------------------
    [AchievementRequirementData]
    [SerializeField] private CollectableData _collectableData;

    public CollectableData CollectableRequirementData
    {
        get
        {
            return _collectableData;
        }
    }
    // -----------------------------------------------------------------------
    // END COLLECTABLE EXTENSION
    // -----------------------------------------------------------------------

    public List<AchievementType> UnlockAfterAchievements
    {
        get
        {
            return _unlockAfterAchievements;
        }
    }

    public bool IsUnlockedAfterAchievement
    {
        get
        {
            return _isUnlockedAfterAchievement;
        }
    }

    public CompletionRequirementType CompletionEnumRequirement
    {
        get
        {
            return _completionRequirement;
        }
    }

    public RewardTier RewardTier
    {
        get
        {
            return _rewardTier;
        }
    }

    public bool IsHidden
    {
        get
        {
            return _progressionData.IsHidden;
        }
    }

    public bool HasProgressionDisplay
    {
        get
        {
            return _progressionData.HasProgressionDisplay;
        }
    }

    public float CurrentAmount
    {
        get
        {
            return _valueData.GetCurrentAmount();
        }
    }

    public bool IsValueGoalReached
    {
        get
        {
            return _valueData.IsRequirementMet();
        }
    }

    public bool IsAchievementGoalReached(Func<string, bool> isUnlocked)
    {
        return _achievementData.IsRequirementMet(isUnlocked);
    }

    public void SetCurrentValue(object value)
    {
        _valueData.SetValue(value);
    }

    // Formats a current/goal pair using this achievement's own display settings.
    // Used by CollectableAchievementBridge to produce the progression string
    // without AchievementType needing to know about collectable types.
    public string FormatProgressionDisplay(float currentAmount, float goalAmount)
    {
        return _progressionData.GetProgressionDisplayType(currentAmount, goalAmount);
    }

    public string GetProgressionDisplay(Func<string, bool> isUnlocked)
    {
        if (_completionRequirement == CompletionRequirementType.NoRequirement)
        {
            return string.Empty;
        }

        switch (_completionRequirement)
        {
            case CompletionRequirementType.ValueRequirement:
                {
                    (float currentAmount, float goalAmount) = _valueData.GetAmountDisplay();
                    return _progressionData.GetProgressionDisplayType(currentAmount, goalAmount);
                }
            case CompletionRequirementType.AchievementRequirement:
                {
                    (int currentAmount, int goalAmount) = _achievementData.GetProgressionDisplay(isUnlocked);
                    return _progressionData.GetProgressionDisplayType(currentAmount, goalAmount);
                }
            default:
                {
                    // Collectable progression is computed by CollectableAchievementBridge
                    // and passed via AchievementSystem.RaiseUIStatus(achievement, progressionDisplay)
                    return string.Empty;
                }
        }
    }

    public void LoadAchievementStatus(AchievementDTO achievementDTO)
    {
        if (achievementDTO == null)
        {
            return;
        }

        if (CompletionEnumRequirement == CompletionRequirementType.ValueRequirement)
        {
            SetCurrentValue(achievementDTO.CurrentAmount);
        }
    }

    public void SaveAchievementStatus(GameData gameData, bool isUnlocked)
    {
        gameData.SetTotalAchievementsData(
            AchievementId,
            Title,
            isUnlocked,
            CurrentAmount
        );
    }
}