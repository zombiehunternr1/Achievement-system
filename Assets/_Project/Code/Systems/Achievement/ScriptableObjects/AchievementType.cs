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
    [AchievementRequirementData]
    [SerializeField] private CollectableData _collectableData;
    [SerializeField] private ValueData _valueData;
    [SerializeField] private bool _isUnlockedAfterAchievement;
    [SerializeField] private List<AchievementType> _unlockAfterAchievements;

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

    // Exposed so the bridge can read collectable requirement data directly.
    // AchievementType itself never references CollectableItem — the bridge handles that.
    public CollectableData CollectableRequirementData
    {
        get
        {
            return _collectableData;
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
                    return GetCollectableProgression();
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

    private string GetCollectableProgression()
    {
        switch (_collectableData.CollectableRequirement)
        {
            case CollectableRequirementType.SingleCollectable:
                {
                    (int currentAmount, int totalAmount) = _collectableData.GetSingleProgressionDisplay();
                    return _progressionData.GetProgressionDisplayType(currentAmount, totalAmount);
                }
            case CollectableRequirementType.AllCollectables:
                {
                    (int currentAmount, int totalAmount) = _collectableData.GetAllProgressionDisplay();
                    return _progressionData.GetProgressionDisplayType(currentAmount, totalAmount);
                }
            default:
                {
                    (int currentAmount, int totalAmount) = _collectableData.GetCustomAmountDisplay();
                    return _progressionData.GetProgressionDisplayType(currentAmount, totalAmount);
                }
        }
    }
}