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
    public float GetCurrentAmount
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
    public bool IsAchievementGoalReached
    {
        get
        {
            return _achievementData.IsRequirementMet();
        }
    }
    public string ProgressionDisplay
    {
        get
        {
            if (_completionRequirement == CompletionRequirementType.NoRequirement)
            {
                return string.Empty;
            }
            switch (_completionRequirement)
            {
                case CompletionRequirementType.ValueRequirement:
                    return GetValueRequirementProgression();
                case CompletionRequirementType.AchievementRequirement:
                    return GetAchievementProgression();
                default:
                    return GetCollectableProgression();
            }
        }
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
    private string GetCustomRequirementProgression()
    {
        (int currentAmount, int totalAmount) = _collectableData.GetCustomAmountDisplay();
        return _progressionData.GetProgressionDisplayType(currentAmount, totalAmount);
    }
    private string GetValueRequirementProgression()
    {
        (float currentAmount, float goalAmount) = _valueData.GetAmountDisplay();
        return _progressionData.GetProgressionDisplayType(currentAmount, goalAmount);
    }
    private string GetAchievementProgression()
    {
        (int currentAmount, int goalAmount) = _achievementData.GetProgressionDisplay();
        return _progressionData.GetProgressionDisplayType(currentAmount, goalAmount);
    }
    private string GetCollectableProgression()
    {
        switch (_collectableData.CollectableRequirement)
        {
            case CollectableRequirementType.SingleCollectable:
                return GetSingleCollectableProgression();
            case CollectableRequirementType.AllCollectables:
                return GetAllCollectablesProgression();
            default:
                return GetCustomRequirementProgression();
        }
    }
    private string GetSingleCollectableProgression()
    {
        (int currentAmount, int totalAmount) = _collectableData.GetSingleProgressionDisplay();
        return _progressionData.GetProgressionDisplayType(currentAmount, totalAmount);
    }
    private string GetAllCollectablesProgression()
    {
        (int currentAmount, int totalAmount) = _collectableData.GetAllProgressionDisplay();
        return _progressionData.GetProgressionDisplayType(currentAmount, totalAmount);
    }
    public void SetCurrentValue(object value)
    {
        _valueData.SetValue(value);
    }
    public void LoadAchievementStatus(AchievementDTO achievementDTO)
    {
        if (achievementDTO.IsUnlocked)
        {
            UnlockAchievement();
        }
        else
        {
            LockAchievement();
        }
        if (CompletionEnumRequirement == CompletionRequirementType.ValueRequirement)
        {
            SetCurrentValue(achievementDTO.CurrentAmount);
        }
    }
    public void SaveAchievementStatus(GameData gameData)
    {
        gameData.SetTotalAchievementsData(
            AchievementId,
            Title,
            IsUnlocked,
            GetCurrentAmount
        );
    }
}
