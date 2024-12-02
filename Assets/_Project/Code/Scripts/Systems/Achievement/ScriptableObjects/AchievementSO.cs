using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "Scriptable Objects/Systems/Achievements/Achievement")]
public class AchievementSO : ScriptableObject
{
    [UniqueIdentifier]
    [SerializeField] private string _achievementId;
    [SerializeField] private string _title;
    [SerializeField] private string _description;
    [SerializeField] private Sprite _icon;
    [SerializeField] private EventReference _soundEffect;
    [SerializeField] private bool _isUnlocked;
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
    public string AchievementId
    {
        get
        {
            return _achievementId;
        }
    }
    public string Title
    {
        get
        {
            return _title;
        }
    }
    public string Description
    {
        get
        {
            return _description;
        }
    }
    public Sprite Icon
    {
        get
        {
            return _icon;
        }
    }
    public CompletionEnumRequirement CompletionEnumRequirement
    {
        get
        {
            return _requirementData.CompletionEnumRequirement;
        }
    }
    public bool IsUnlocked
    {
        get
        {
            return _isUnlocked;
        }
    }
    public EventReference SoundEffect
    {
        get
        {
            return _soundEffect;
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
            if (_valueData.ValueEnumType == ValueEnumType.Integer)
            {
                return _valueData.CurrentIntegerAmount;
            }
            return _valueData.CurrentFloatAmount;
        }
    }
    private int GetActualAchievementCount
    {
        get
        {
            int amount = 0;
            for (int i = 0; i < _achievementData.AchievementListReference.AchievementList.Count; i++)
            {
                AchievementSO achievement = _achievementData.AchievementListReference.AchievementList[i];
                if (achievement != null && achievement.CompletionEnumRequirement != CompletionEnumRequirement.AchievementRequirement)
                {
                    amount++;
                }
            }
            return amount;
        }
    }
    public bool IsValueGoalReached
    {
        get
        {
            if (_valueData.ValueEnumType == ValueEnumType.Integer)
            {
                if (_valueData.IsExactAmount)
                {
                    return _valueData.CurrentIntegerAmount == _valueData.GoalIntegerAmount;
                }
                if (_valueData.CurrentIntegerAmount >= _valueData.GoalIntegerAmount)
                {
                    _valueData.SetToIntegerGoalAmount();
                    return true;
                }
            }
            if (_valueData.IsExactAmount)
            {
                return _valueData.CurrentFloatAmount == _valueData.GoalFloatAmount;
            }
            if (_valueData.CurrentFloatAmount >= _valueData.GoalFloatAmount)
            {
                _valueData.SetToFloatGoalAmount();
                return true;
            }
            return false;
        }
    }
    public bool IsAchievementGoalReached
    {
        get
        {
            int currentAmount = 0;
            int goalAmount;
            if (_achievementData.HasCustomGoalAmount)
            {
                goalAmount = _achievementData.GoalAmount;
            }
            else
            {
                goalAmount = GetActualAchievementCount;
            }
            for (int i = 0; i < _achievementData.AchievementListReference.AchievementList.Count; i++)
            {
                AchievementSO achievement = _achievementData.AchievementListReference.AchievementList[i];
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
    }
    public string GetProgressionDisplay
    {
        get
        {
            if (_requirementData.CompletionEnumRequirement == CompletionEnumRequirement.NoRequirement)
            {
                return string.Empty;
            }
            if (_requirementData.CompletionEnumRequirement == CompletionEnumRequirement.ValueRequirement)
            {
                return GetValueRequirementProgression();
            }
            if (_requirementData.CompletionEnumRequirement == CompletionEnumRequirement.AchievementRequirement)
            {
                return GetAchievementProgression();
            }
            if (_collectableData.CollectableEnumRequirement == CollectableEnumRequirement.SingleCollectable)
            {
                return GetSingleCollectableProgression();
            }
            if (_collectableData.CollectableEnumRequirement == CollectableEnumRequirement.AllCollectables)
            {
                return GetAllCollectablesProgression();
            }
            return GetCustomRequirementProgression();
        }
    }
    public bool IsCollectableGoalReached(CollectableTypeSO collectable)
    {
        if (_collectableData.CollectableEnumRequirement == CollectableEnumRequirement.SingleCollectable)
        {
            return IsItemRequirementMet(collectable);
        }
        if (_collectableData.CollectableEnumRequirement == CollectableEnumRequirement.AllCollectables)
        {
            return IsAllCollectablesRequirementMet();
        }
        if (_collectableData.CollectableEnumRequirement == CollectableEnumRequirement.Custom)
        {
            return IsCustomGoalRequirementMet(_collectableData.MinimumGoalAmount);
        }
        return false;
    }
    public bool IsAchievementRelated(CollectableSO collectable)
    {
        if (_requirementData.CompletionEnumRequirement == CompletionEnumRequirement.NoRequirement ||
            _requirementData.CompletionEnumRequirement == CompletionEnumRequirement.ValueRequirement)
        {
            return false;
        }
        if (_collectableData.CollectableEnumRequirement == CollectableEnumRequirement.SingleCollectable)
        {
            return _collectableData.CollectableReference != null && _collectableData.CollectableReference.IsMatchingId(collectable.CollectableId);
        }
        for (int i = 0; i < _collectableData.CollectableListReference.CollectablesList.Count; i++)
        {
            if (_collectableData.CollectableListReference.CollectablesList.Contains(collectable))
            {
                return true;
            }
        }
        return false;
    }
    private Dictionary<CollectableCategoryEnum, int> GetCollectedAmountPerCategory()
    {
        Dictionary<CollectableCategoryEnum, int> collectedAmountPerCategory = new Dictionary<CollectableCategoryEnum, int>();
        for (int i = 0; i < _collectableData.CollectableListReference.CollectablesList.Count; i++)
        {
            if (_collectableData.CollectableListReference.CollectablesList[i].CollectableCategory == CollectableCategoryEnum.None)
            {
                continue;
            }
            if (!collectedAmountPerCategory.ContainsKey(_collectableData.CollectableListReference.CollectablesList[i].CollectableCategory))
            {
                collectedAmountPerCategory[_collectableData.CollectableListReference.CollectablesList[i].CollectableCategory] = 0;
            }
        }
        foreach (CollectableSO collectable in _collectableData.CollectableListReference.CollectablesList)
        {
            if (collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem && collectable.IsCollected)
            {
                collectedAmountPerCategory[collectable.CollectableCategory]++;
            }
            for (int i = 0; i < collectable.MultiCollectables; i++)
            {
                if (collectable.IsCollectedFromList(i))
                {
                    collectedAmountPerCategory[collectable.CollectableCategory]++;
                }
            }
        }
        return collectedAmountPerCategory;
    }
    private bool IsItemRequirementMet(CollectableTypeSO collectable)
    {
        if (_collectableData.CollectableReference.ItemAmountType == CollectionEnumItemAmount.SingleItem)
        {
            return _collectableData.CollectableReference.IsMatchingId(collectable.CollectableId) && _collectableData.CollectableReference.IsCollected;
        }
        for (int i = 0; i < _collectableData.CollectableReference.MultiCollectables; i++)
        {
            if (!_collectableData.CollectableReference.IsMatchingIdInList(i, collectable.CollectableIdFromList(i)) ||
                !_collectableData.CollectableReference.IsCollectedFromList(i))
            {
                return false;
            }
        }
        return true;
    }
    private bool IsAllCollectablesRequirementMet()
    {
        for (int i = 0; i < _collectableData.CollectableListReference.CollectablesList.Count; i++)
        {
            if (_collectableData.CollectableListReference.CollectablesList[i].CollectableCategory == CollectableCategoryEnum.None)
            {
                continue;
            }
            if (_collectableData.CollectableListReference.CollectablesList[i].ItemAmountType == CollectionEnumItemAmount.SingleItem)
            {
                if (!_collectableData.CollectableListReference.CollectablesList[i].IsCollected)
                {
                    return false;
                }
            }
            for (int j = 0; j < _collectableData.CollectableListReference.CollectablesList[i].MultiCollectables; j++)
            {
                if (!_collectableData.CollectableListReference.CollectablesList[i].IsCollectedFromList(j))
                {
                    return false;
                }
            }
        }
        return true;
    }
    private bool IsCustomGoalRequirementMet(int goalAmount)
    {
        Dictionary<CollectableCategoryEnum, int> collectedAmountPerCategory = GetCollectedAmountPerCategory();
        foreach (int collectableAmount in collectedAmountPerCategory.Values)
        {
            if (collectableAmount < goalAmount)
            {
                return false;
            }
        }
        return true;
    }
    private string GetProgressionDisplayType(float currentAmount, float goalAmount)
    {
        if (_progressionData.ProgressionEnumDisplay == ProgressionEnumDisplay.FullAmount)
        {
            return currentAmount + " / " + goalAmount;
        }
        float percentageAmount;
        percentageAmount = Mathf.InverseLerp(0, goalAmount, currentAmount) * 100;
        return percentageAmount + "%";
    }
    private string GetCustomRequirementProgression()
    {
        int currentAmount = GetCollectedAmount(_collectableData.MinimumGoalAmount);
        int totalAmount = GetUniqueCategoryCount();
        return GetProgressionDisplayType(currentAmount, totalAmount);
    }
    private string GetValueRequirementProgression()
    {
        if (_valueData.ValueEnumType == ValueEnumType.Integer)
        {
           return GetProgressionDisplayType(_valueData.CurrentIntegerAmount, _valueData.GoalIntegerAmount);
        }
        return GetProgressionDisplayType(_valueData.CurrentFloatAmount, _valueData.GoalFloatAmount);
    }
    private string GetAchievementProgression()
    {
        int currentAmount = 0;
        int goalAmount;
        if (_achievementData.HasCustomGoalAmount)
        {
            goalAmount = _achievementData.GoalAmount;
        }
        else
        {
            goalAmount = GetActualAchievementCount;
        }
        foreach (AchievementSO achievement in _achievementData.AchievementListReference.AchievementList)
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
        return GetProgressionDisplayType(currentAmount, goalAmount);
    }
    private string GetSingleCollectableProgression()
    {
        int currentAmount = 0;
        int totalAmount;
        if (_collectableData.CollectableReference.ItemAmountType == CollectionEnumItemAmount.SingleItem)
        {
            totalAmount = 1;
            if (_collectableData.CollectableReference.IsCollected)
            {
                currentAmount = 1;
            }
        }
        else
        {
            totalAmount = _collectableData.CollectableReference.MultiCollectables;
            for (int i = 0; i < totalAmount; i++)
            {
                if (_collectableData.CollectableReference.IsCollectedFromList(i))
                {
                    currentAmount++;
                }
            }
        }
        return GetProgressionDisplayType(currentAmount, totalAmount);
    }
    private string GetAllCollectablesProgression()
    {
        int currentAmount = 0;
        int totalAmount = 0;
        foreach (CollectableSO collectable in _collectableData.CollectableListReference.CollectablesList)
        {
            if (collectable.CollectableCategory == CollectableCategoryEnum.None)
            {
                continue;
            }
            if (collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem)
            {
                if (collectable.IsCollected)
                {
                    currentAmount++;
                }
                totalAmount++;
            }
            for (int j = 0; j < collectable.MultiCollectables; j++)
            {
                if (collectable.IsCollectedFromList(j))
                {
                    currentAmount++;
                }
                totalAmount++;
            }
        }
        return GetProgressionDisplayType(currentAmount, totalAmount);
    }
    private int GetUniqueCategoryCount()
    {
        HashSet<CollectableCategoryEnum> uniqueCategories = new HashSet<CollectableCategoryEnum>();
        for (int i = 0; i < _collectableData.CollectableListReference.CollectablesList.Count; i++)
        {
            CollectableCategoryEnum category = _collectableData.CollectableListReference.CollectablesList[i].CollectableCategory;
            if (category == CollectableCategoryEnum.None)
            {
                continue;
            }
            if (!uniqueCategories.Contains(category))
            {
                uniqueCategories.Add(category);
            }
        }
        return uniqueCategories.Count;
    }
    private int GetCollectedAmount(int goalAmount)
    {
        Dictionary<CollectableCategoryEnum, int> collectedAmountPerCategory = GetCollectedAmountPerCategory();
        int totalCollected = 0;
        foreach (int categoryAmount in collectedAmountPerCategory.Values)
        {
            if (categoryAmount >= goalAmount)
            {
                totalCollected++;
            }
        }
        return totalCollected;
    }
    public void UnlockAchievement()
    {
        _isUnlocked = true;
    }
    public void LockAchievement()
    {
        _isUnlocked = false;
    }
    public void NewCurrentValue(object value)
    {
        _valueData.SetNewValue(value);
    }
}
