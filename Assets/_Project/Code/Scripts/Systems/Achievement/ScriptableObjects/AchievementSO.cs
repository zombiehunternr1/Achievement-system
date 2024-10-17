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
    [SerializeField] private CollectableTypeSO _collectableType;
    [SerializeField] private CollectableTypeListSO _collectableTypeList;
    [SerializeField] private List<CollectableTypeListSO> _collectableTypeLists;
    [SerializeField] private bool _requiresMultipleCollectableLists;
    [Tooltip("This value is for the minimum amount required in a list or that needs to be met in multiple lists")]
    [SerializeField] private int _minimumGoalAmount;
    public AchievementSO PreviousAchievementSO
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
            if(_isExactAmount)
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
    public bool IsCollectableGoalReached(BaseCollectableTypeSO collectable)
    {
        if (_collectableEnumRequirement == CollectableEnumRequirement.SingleCollectable)
        {
            return _collectableType != null && (_collectableType.CollectableId.Equals(collectable.CollectableId) && _collectableType.IsCollected);
        }
        if (_collectableEnumRequirement == CollectableEnumRequirement.AllCollectables)
        {
            return _collectableTypeList.CollectablesList.Contains(collectable) && 
                IsRequirementMet(_collectableTypeList.CollectablesList, _collectableTypeList.CollectablesList.Count,
                _requiresMultipleCollectableLists);
        }
        if (_collectableEnumRequirement == CollectableEnumRequirement.Custom)
        {
            if (!_requiresMultipleCollectableLists)
            {
                return _collectableTypeList.CollectablesList.Contains(collectable) && 
                    IsRequirementMet(_collectableTypeList.CollectablesList, _minimumGoalAmount, _requiresMultipleCollectableLists);
            }
            for (int i = 0; i < _collectableTypeLists.Count; i++)
            {
                if (_collectableTypeLists[i].CollectablesList.Contains(collectable) && 
                    IsRequirementMet(_collectableTypeLists[i].CollectablesList, _minimumGoalAmount, _requiresMultipleCollectableLists))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool IsAchievementRelated(string collectableId)
    {
        if (_completionEnumRequirement == CompletionEnumRequirement.NoRequirement || 
            _completionEnumRequirement == CompletionEnumRequirement.ValueRequirement)
        {
            return false;
        }
        if (_collectableEnumRequirement == CollectableEnumRequirement.SingleCollectable)
        {
            return _collectableType != null && _collectableType.CollectableId.Equals(collectableId);
        }
        if (!_requiresMultipleCollectableLists)
        {
            for (int i = 0; i < _collectableTypeList.CollectablesList.Count; i++)
            {
                if (_collectableTypeList.CollectablesList[i].CollectableId == collectableId)
                {
                    return true;
                }
            }
        }
        for (int i = 0; i < _collectableTypeLists.Count; i++)
        {
            for (int j = 0; j < _collectableTypeLists[i].CollectablesList.Count; j++)
            {
                if (_collectableTypeLists[i].CollectablesList[j].CollectableId == collectableId)
                {
                    return true;
                }
            }
        }
        return false;
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
    private bool IsRequirementMet(List<BaseCollectableTypeSO> collectablesList, int goalAmount, bool isMultipleLists)
    {
        int currentAmount;
        if (!isMultipleLists)
        {
            currentAmount = GetCollectedAmount(collectablesList, goalAmount);
            return currentAmount >= goalAmount;
        }
        int totalAmount = 0;
        for (int i = 0; i < _collectableTypeLists.Count; i++)
        {
            currentAmount = GetCollectedAmount(_collectableTypeLists[i].CollectablesList, _minimumGoalAmount);
            totalAmount += currentAmount;
            if (totalAmount >= _collectableTypeLists.Count * _minimumGoalAmount)
            {
                return true;
            }
        }
        return false;
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
        int totalAmount = 0;
        int currentAmount;
        if (!_requiresMultipleCollectableLists)
        {
            currentAmount = GetCollectedAmount(_collectableTypeList.CollectablesList, _minimumGoalAmount);
            return GetProgressionDisplayType(currentAmount, _minimumGoalAmount);
        }
        for (int i = 0; i < _collectableTypeLists.Count; i++)
        {
            currentAmount = GetCollectedAmount(_collectableTypeLists[i].CollectablesList, _minimumGoalAmount);
            totalAmount += currentAmount;
            if (totalAmount >= _collectableTypeLists.Count * _minimumGoalAmount)
            {
                break;
            }
        }
        return GetProgressionDisplayType(totalAmount, _collectableTypeLists.Count * _minimumGoalAmount);
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
        if (_collectableType.IsCollected)
        {
            currentAmount++;
        }
        return GetProgressionDisplayType(currentAmount, 1);
    }
    private string GetAllCollectablesProgression()
    {
        int currentAmount = 0;
        for (int i = 0; i < _collectableTypeList.CollectablesList.Count; i++)
        {
            if (_collectableTypeList.CollectablesList[i].IsCollected)
            {
                currentAmount++;
            }
        }
        return GetProgressionDisplayType(currentAmount, _collectableTypeList.CollectablesList.Count);
    }
    private int GetCollectedAmount(List<BaseCollectableTypeSO> collectablesList, int goalAmount)
    {
        int count = 0;
        for (int i = 0; i < collectablesList.Count; i++)
        {
            if (collectablesList[i].IsCollected)
            {
                count++;
                if (count == goalAmount)
                {
                    break;
                }
            }
        }
        return count;
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
