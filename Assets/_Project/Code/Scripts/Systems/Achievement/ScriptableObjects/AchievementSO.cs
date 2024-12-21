using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "Scriptable Objects/Systems/Achievements/Achievement")]
public class AchievementSO : AchievementTypeSO
{
    [SerializeField] private RequirementData _requirementData;
    [SerializeField] private ProgressionData _progressionData;
    [SerializeField] private AchievementData _achievementData;
    [SerializeField] private CollectableData _collectableData;
    [SerializeField] private ValueData _valueData;
    public AchievementSO PreviousAchievement
    {
        get
        {
            return _requirementData.PreviousAchievementReference;
        }
    }
    public bool RequiresPreviousAchievement
    {
        get
        {
            return _requirementData.RequiresPreviousAchievementToUnlock;
        }
    }
    public CompletionEnumRequirement CompletionEnumRequirement
    {
        get
        {
            return _requirementData.CompletionEnumRequirement;
        }
    }
    public bool IsPreviousAchievementUnlocked
    {
        get
        {
            return _requirementData.PreviousAchievementReference.IsUnlocked;
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
    public string GetProgressionDisplay
    {
        get
        {
            if (_requirementData.CompletionEnumRequirement == CompletionEnumRequirement.NoRequirement)
            {
                return string.Empty;
            }
            switch (_requirementData.CompletionEnumRequirement)
            {
                case CompletionEnumRequirement.ValueRequirement:
                    return GetValueRequirementProgression();
                case CompletionEnumRequirement.AchievementRequirement:
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
        if (_requirementData.CompletionEnumRequirement == CompletionEnumRequirement.NoRequirement ||
            _requirementData.CompletionEnumRequirement == CompletionEnumRequirement.ValueRequirement)
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
        switch (_collectableData.CollectableEnumRequirement)
        {
            case CollectableEnumRequirement.SingleCollectable:
                return GetSingleCollectableProgression();
            case CollectableEnumRequirement.AllCollectables:
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
        if (CompletionEnumRequirement == CompletionEnumRequirement.ValueRequirement)
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
