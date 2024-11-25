using FMODUnity;
using System;
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
    [SerializeField] private bool _unlocked;

    [SerializeField] private bool _requiresPreviousAchievement;
    [SerializeField] private AchievementSO _previousAchievementReference;
    [SerializeField] private AchievementSOList _achievementListReference;
    [SerializeField] private bool _customAchievementGoalAmount;
    [SerializeField] private int _goalAchievementAmount;

    [SerializeField] private bool _isHidden;
    [SerializeField] private bool _showProgression;
    [SerializeField] private ProgressionEnumDisplay _progressionEnumDisplay;

    [SerializeField] private CompletionEnumRequirement _completionEnumRequirement;
    [SerializeField] private ValueEnumType _valueEnumType;
    [SerializeField] private bool _isExactAmount;
    [SerializeField] private int _currentIntegerAmount;
    [SerializeField] private int _goalIntegerAmount;
    [SerializeField] private float _currentFloatAmount;
    [SerializeField] private float _goalFloatAmount;

    [SerializeField] private CollectableEnumRequirement _collectableEnumRequirement;
    [SerializeField] private CollectableSO _collectableReference;
    [SerializeField] private CollectableListSO _collectableListReference;
    [Tooltip("This value is for the minimum amount required per collectable type in the list")]
    [SerializeField] private int _minimumGoalAmount;
    public AchievementSO PreviousAchievement
    {
        get
        {
            return _previousAchievementReference;
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
            return _completionEnumRequirement;
        }
    }
    public bool IsUnlocked
    {
        get
        {
            return _unlocked;
        }
    }
    public EventReference SoundEffect
    {
        get
        {
            return _soundEffect;
        }
    }
    public bool RequiresPreviousAchievement
    {
        get
        {
            return _requiresPreviousAchievement;
        }
    }
    public bool PreviousAchievementUnlocked
    {
        get
        {
            return _previousAchievementReference.IsUnlocked;
        }
    }
    public bool IsHidden
    {
        get
        {
            return _isHidden;
        }
    }
    public bool ShowProgression
    {
        get
        {
            return _showProgression;
        }
    }
    public float GetCurrentAmount
    {
        get
        {
            if (_valueEnumType == ValueEnumType.Integer)
            {
                return _currentIntegerAmount;
            }
            return _currentFloatAmount;
        }
    }
    private int ActualAchievementCount
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
    public bool IsValueGoalReached
    {
        get
        {
            if (_valueEnumType == ValueEnumType.Integer)
            {
                if (_isExactAmount)
                {
                    return _currentIntegerAmount == _goalIntegerAmount;
                }
                if (_currentIntegerAmount >= _goalIntegerAmount)
                {
                    _currentIntegerAmount = _goalIntegerAmount;
                    return true;
                }
            }
            if (_isExactAmount)
            {
                return _currentFloatAmount == _goalFloatAmount;
            }
            if (_currentFloatAmount >= _goalFloatAmount)
            {
                _currentFloatAmount = _goalFloatAmount;
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
            if (_customAchievementGoalAmount)
            {
                goalAmount = _goalAchievementAmount;
            }
            else
            {
                goalAmount = ActualAchievementCount;
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
    }
    public string Progression
    {
        get
        {
            if (_completionEnumRequirement == CompletionEnumRequirement.NoRequirement)
            {
                return string.Empty;
            }
            if (_completionEnumRequirement == CompletionEnumRequirement.ValueRequirement)
            {
                return GetValueRequirementProgression();
            }
            if (_completionEnumRequirement == CompletionEnumRequirement.AchievementRequirement)
            {
                return GetAchievementProgression();
            }
            if (_collectableEnumRequirement == CollectableEnumRequirement.SingleCollectable)
            {
                return GetSingleCollectableProgression();
            }
            if (_collectableEnumRequirement == CollectableEnumRequirement.AllCollectables)
            {
                return GetAllCollectablesProgression();
            }
            return GetCustomRequirementProgression();
        }
    }
    public bool IsCollectableGoalReached(CollectableTypeSO collectable)
    {
        if (_collectableEnumRequirement == CollectableEnumRequirement.SingleCollectable)
        {
            return IsItemRequirementMet(collectable);
        }
        if (_collectableEnumRequirement == CollectableEnumRequirement.AllCollectables)
        {
            return IsAllCollectablesRequirementMet();
        }
        if (_collectableEnumRequirement == CollectableEnumRequirement.Custom)
        {
            return IsCustomGoalRequirementMet(_minimumGoalAmount);
        }
        return false;
    }
    public bool IsAchievementRelated(CollectableSO collectable)
    {
        if (_completionEnumRequirement == CompletionEnumRequirement.NoRequirement ||
            _completionEnumRequirement == CompletionEnumRequirement.ValueRequirement)
        {
            return false;
        }
        if (_collectableEnumRequirement == CollectableEnumRequirement.SingleCollectable)
        {
            return _collectableReference != null && _collectableReference.IsMatchingId(collectable.CollectableId());
        }
        for (int i = 0; i < _collectableListReference.CollectablesList.Count; i++)
        {
            if (_collectableListReference.CollectablesList.Contains(collectable))
            {
                return true;
            }
        }
        return false;
    }
    private Dictionary<CollectableCategoryEnum, int> GetCollectedAmountPerCategory()
    {
        Dictionary<CollectableCategoryEnum, int> collectedAmountPerCategory = new Dictionary<CollectableCategoryEnum, int>();
        for (int i = 0; i < _collectableListReference.CollectablesList.Count; i++)
        {
            if (_collectableListReference.CollectablesList[i].CollectableCategory == CollectableCategoryEnum.None)
            {
                continue;
            }
            if (!collectedAmountPerCategory.ContainsKey(_collectableListReference.CollectablesList[i].CollectableCategory))
            {
                collectedAmountPerCategory[_collectableListReference.CollectablesList[i].CollectableCategory] = 0;
            }
        }
        foreach (CollectableSO collectable in _collectableListReference.CollectablesList)
        {
            if (collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem && collectable.IsCollected())
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
        if (_collectableReference.ItemAmountType == CollectionEnumItemAmount.SingleItem)
        {
            return _collectableReference.IsMatchingId(collectable.CollectableId()) && _collectableReference.IsCollected();
        }
        for (int i = 0; i < _collectableReference.MultiCollectables; i++)
        {
            if (!_collectableReference.IsMatchingIdInList(i, collectable.CollectableIdFromList(i)) ||
                !_collectableReference.IsCollectedFromList(i))
            {
                return false;
            }
        }
        return true;
    }
    private bool IsAllCollectablesRequirementMet()
    {
        for (int i = 0; i < _collectableListReference.CollectablesList.Count; i++)
        {
            if (_collectableListReference.CollectablesList[i].CollectableCategory == CollectableCategoryEnum.None)
            {
                continue;
            }
            if (_collectableListReference.CollectablesList[i].ItemAmountType == CollectionEnumItemAmount.SingleItem)
            {
                if (!_collectableListReference.CollectablesList[i].IsCollected())
                {
                    return false;
                }
            }
            for (int j = 0; j < _collectableListReference.CollectablesList[i].MultiCollectables; j++)
            {
                if (!_collectableListReference.CollectablesList[i].IsCollectedFromList(j))
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
        if (_progressionEnumDisplay == ProgressionEnumDisplay.FullAmount)
        {
            return currentAmount + " / " + goalAmount;
        }
        float percentageAmount;
        percentageAmount = Mathf.InverseLerp(0, goalAmount, currentAmount) * 100;
        return percentageAmount + "%";
    }
    private string GetCustomRequirementProgression()
    {
        int currentAmount = GetCollectedAmount(_minimumGoalAmount);
        int totalAmount = GetUniqueCategoryCount();
        return GetProgressionDisplayType(currentAmount, totalAmount);
    }
    private string GetValueRequirementProgression()
    {
        if (_valueEnumType == ValueEnumType.Integer)
        {
           return GetProgressionDisplayType(_currentIntegerAmount, _goalIntegerAmount);
        }
        return GetProgressionDisplayType(_currentFloatAmount, _goalFloatAmount);
    }
    private string GetAchievementProgression()
    {
        int currentAmount = 0;
        int goalAmount;
        if (_customAchievementGoalAmount)
        {
            goalAmount = _goalAchievementAmount;
        }
        else
        {
            goalAmount = ActualAchievementCount;
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
        return GetProgressionDisplayType(currentAmount, goalAmount);
    }
    private string GetSingleCollectableProgression()
    {
        int currentAmount = 0;
        int totalAmount;
        if (_collectableReference.ItemAmountType == CollectionEnumItemAmount.SingleItem)
        {
            totalAmount = 1;
            if (_collectableReference.IsCollected())
            {
                currentAmount = 1;
            }
        }
        else
        {
            totalAmount = _collectableReference.MultiCollectables;
            for (int i = 0; i < totalAmount; i++)
            {
                if (_collectableReference.IsCollectedFromList(i))
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
        foreach (CollectableSO collectable in _collectableListReference.CollectablesList)
        {
            if (collectable.CollectableCategory == CollectableCategoryEnum.None)
            {
                continue;
            }
            if (collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem)
            {
                if (collectable.IsCollected())
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
        for (int i = 0; i < _collectableListReference.CollectablesList.Count; i++)
        {
            CollectableCategoryEnum category = _collectableListReference.CollectablesList[i].CollectableCategory;
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
        _unlocked = true;
    }
    public void LockAchievement()
    {
        _unlocked = false;
    }
    public void NewCurrentValue(object value)
    {
        if (_valueEnumType == ValueEnumType.Integer)
        {
            _currentIntegerAmount = Convert.ToInt32(value);
        }
        else
        {          
            _currentFloatAmount = Convert.ToSingle(value);
        }
    }
}
