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
    [SerializeField] private AchievementSO _previousAchievement;
    [SerializeField] private AchievementSOList _achievementList;
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
    [SerializeField] private CollectableSO _collectable;
    [SerializeField] private CollectableListSO _collectableList;
    [Tooltip("This value is for the minimum amount required in a list or that needs to be met in multiple lists")]
    [SerializeField] private int _minimumGoalAmount;
    public AchievementSO PreviousAchievement
    {
        get
        {
            return _previousAchievement;
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
            return _previousAchievement.IsUnlocked;
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
            for (int i = 0; i < _achievementList.AchievementList.Count; i++)
            {
                AchievementSO achievement = _achievementList.AchievementList[i];
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
            for (int i = 0; i < _achievementList.AchievementList.Count; i++)
            {
                AchievementSO achievement = _achievementList.AchievementList[i];
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
            return _collectable != null && (_collectable.SingleCollectableStatus.CollectableId.Equals(collectable.SingleCollectableStatus.CollectableId) &&
                _collectable.SingleCollectableStatus.IsCollected);
        }
        if (_collectableEnumRequirement == CollectableEnumRequirement.AllCollectables)
        {
            return IsRequirementMet(_collectableList);
        }
        if (_collectableEnumRequirement == CollectableEnumRequirement.Custom)
        {
            return IsRequirementMet(_collectableList, _minimumGoalAmount);
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
            return _collectable != null && _collectable.SingleCollectableStatus.CollectableId.Equals(collectable.SingleCollectableStatus.CollectableId);
        }
        for (int i = 0; i < _collectableList.CollectablesList.Count; i++)
        {
            if (_collectableList.CollectablesList.Contains(collectable))
            {
                return true;
            }
        }
        return false;
    }
    private Dictionary<CollectableCategoryEnum, int> GetCollectedAmountPerCategory(List<CollectableSO> singleItems, List<CollectableSO> multipleItems)
    {
        Dictionary<CollectableCategoryEnum, int> collectedAmountPerCategory = new Dictionary<CollectableCategoryEnum, int>();
        for (int i = 0; i < singleItems.Count; i++)
        {
            if (!collectedAmountPerCategory.ContainsKey(singleItems[i].CollectableCategory))
            {
                collectedAmountPerCategory[singleItems[i].CollectableCategory] = 0;
            }
        }
        for (int i = 0; i < multipleItems.Count; i++)
        {
            if (!collectedAmountPerCategory.ContainsKey(multipleItems[i].CollectableCategory))
            {
                collectedAmountPerCategory[multipleItems[i].CollectableCategory] = 0;
            }
        }
        foreach (CollectableSO collectable in singleItems)
        {
            if (collectable.SingleCollectableStatus.IsCollected)
            {
                collectedAmountPerCategory[collectable.CollectableCategory]++;
            }
        }
        foreach (CollectableSO collectable in multipleItems)
        {
            for (int i = 0; i < collectable.MultiCollectableStatus.Count; i++)
            {
                if (collectable.MultiCollectableStatus[i].IsCollected)
                {
                    collectedAmountPerCategory[collectable.CollectableCategory]++;
                }
            }
        }
        return collectedAmountPerCategory;
    }
    private bool IsRequirementMet(CollectableListSO collectablesList)
    {
        List<CollectableSO> singleItemsList = collectablesList.SingleItems;
        List<CollectableSO> multipleItemsList = collectablesList.MultipleItems;
        for (int i = 0; i < singleItemsList.Count; i++)
        {
            if (!singleItemsList[i].SingleCollectableStatus.IsCollected)
            {
                return false;
            }
        }
        for (int i = 0; i < multipleItemsList.Count; i++)
        {
            for (int j = 0; j < multipleItemsList[i].MultiCollectableStatus.Count; j++)
            {
                if (!multipleItemsList[i].MultiCollectableStatus[j].IsCollected)
                {
                    return false;
                }
            }
        }
        return true;
    }
    private bool IsRequirementMet(CollectableListSO collectablesList, int goalAmount)
    {
        Dictionary<CollectableCategoryEnum, int> collectedAmountPerCategory = GetCollectedAmountPerCategory(collectablesList.SingleItems, collectablesList.MultipleItems);
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
        int currentAmount = GetCollectedAmount(_collectableList, _minimumGoalAmount);
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
        for (int i = 0; i < _achievementList.AchievementList.Count; i++)
        {
            AchievementSO achievement = _achievementList.AchievementList[i];
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
        if (_collectable.SingleCollectableStatus.IsCollected)
        {
            currentAmount++;
        }
        return GetProgressionDisplayType(currentAmount, 1);
    }
    private string GetAllCollectablesProgression()
    {
        int currentAmount = 0;
        int totalAmount = 0;
        for (int i = 0; i < _collectableList.SingleItems.Count; i++)
        {
            if (_collectableList.SingleItems[i].SingleCollectableStatus.IsCollected)
            {
                currentAmount++;
            }
            totalAmount++;
        }
        for (int i = 0; i < _collectableList.MultipleItems.Count; i++)
        {
            for (int j = 0; j < _collectableList.MultipleItems[i].MultiCollectableStatus.Count; j++)
            {
                if (_collectableList.MultipleItems[i].MultiCollectableStatus[j].IsCollected)
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
        for (int i = 0; i < _collectableList.SingleItems.Count; i++)
        {
            if (!uniqueCategories.Contains(_collectableList.SingleItems[i].CollectableCategory))
            {
                uniqueCategories.Add(_collectableList.SingleItems[i].CollectableCategory);
            }
        }
        for (int i = 0; i < _collectableList.MultipleItems.Count; i ++)
        {
            if (!uniqueCategories.Contains(_collectableList.MultipleItems[i].CollectableCategory))
            {
                uniqueCategories.Add(_collectableList.MultipleItems[i].CollectableCategory);
            }
        }
        return uniqueCategories.Count;
    }
    private int GetCollectedAmount(CollectableListSO collectablesList, int goalAmount)
    {
        Dictionary<CollectableCategoryEnum, int> collectedAmountPerCategory = GetCollectedAmountPerCategory(collectablesList.SingleItems, collectablesList.MultipleItems);
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
