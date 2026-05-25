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
    [SerializeField] private CollectableData _collectableData;
    [SerializeField] private ValueData _valueData;
    [SerializeField] private bool _isUnlockedAfterAchievement;
    [SerializeField] private List<AchievementType> _unlockAfterAchievements;

    public List<AchievementType> UnlockAfterAchievements
    {
        get { return _unlockAfterAchievements; }
    }

    public bool IsUnlockedAfterAchievement
    {
        get { return _isUnlockedAfterAchievement; }
    }

    public CompletionRequirementType CompletionEnumRequirement
    {
        get { return _completionRequirement; }
    }

    public RewardTier RewardTier
    {
        get { return _rewardTier; }
    }

    public bool IsHidden
    {
        get { return _progressionData.IsHidden; }
    }

    public bool HasProgressionDisplay
    {
        get { return _progressionData.HasProgressionDisplay; }
    }

    public float CurrentAmount
    {
        get { return _valueData.GetCurrentAmount(); }
    }

    public bool IsValueGoalReached
    {
        get { return _valueData.IsRequirementMet(); }
    }

    public bool IsAchievementGoalReached(Func<string, bool> isUnlocked)
    {
        return _achievementData.IsRequirementMet(isUnlocked);
    }

    public bool IsCollectableGoalReached(CollectableItem collectable)
    {
        return _collectableData.IsRequirementMet(collectable);
    }

    public bool IsAchievementRelated(CollectableItem collectable)
    {
        if (_completionRequirement == CompletionRequirementType.NoRequirement ||
            _completionRequirement == CompletionRequirementType.ValueRequirement)
        {
            return false;
        }

        return _collectableData.IsRelatedToAchievement(collectable);
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
                (float currentVal, float goalVal) = _valueData.GetAmountDisplay();
                return _progressionData.GetProgressionDisplayType(currentVal, goalVal);

            case CompletionRequirementType.AchievementRequirement:
                (int currentAch, int goalAch) = _achievementData.GetProgressionDisplay(isUnlocked);
                return _progressionData.GetProgressionDisplayType(currentAch, goalAch);

            default:
                return GetCollectableProgression();
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

    private string GetCustomRequirementProgression()
    {
        (int currentAmount, int totalAmount) = _collectableData.GetCustomAmountDisplay();
        return _progressionData.GetProgressionDisplayType(currentAmount, totalAmount);
    }

    private string GetCollectableProgression()
    {
        switch (_collectableData.CollectableRequirement)
        {
            case CollectableRequirementType.SingleCollectable:
                (int singleCurrent, int singleTotal) = _collectableData.GetSingleProgressionDisplay();
                return _progressionData.GetProgressionDisplayType(singleCurrent, singleTotal);

            case CollectableRequirementType.AllCollectables:
                (int allCurrent, int allTotal) = _collectableData.GetAllProgressionDisplay();
                return _progressionData.GetProgressionDisplayType(allCurrent, allTotal);

            default:
                return GetCustomRequirementProgression();
        }
    }
}