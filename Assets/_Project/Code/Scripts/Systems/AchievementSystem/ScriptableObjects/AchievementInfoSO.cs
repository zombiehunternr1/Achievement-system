using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "Scriptable Objects/Achievements/Achievement")]
public class AchievementInfoSO : ScriptableObject
{
    [UniqueIdentifier]
    [SerializeField] private string _achievementId;
    [SerializeField] private string _title;
    [SerializeField] private string _description;
    [SerializeField] private Sprite _icon;
    public enum CompletionEnumType { NoRequirements, IntegerRequirement, FloatRequirement };
    [SerializeField] private CompletionEnumType _completionType;
    public enum CollectableEnumType { None, Collectable, Achievement };
    [SerializeField] private CollectableEnumType _collectableType;
    public enum CollectableRequirementEnumType { Single, List};
    [SerializeField] private CollectableRequirementEnumType _collectableRequirementType;

    [SerializeField] private int _intCurrentAmount;
    [SerializeField] private int _intGoalAmount;
    [SerializeField] private float _floatCurrentAmount;
    [SerializeField] private float _floatGoalAmount;
    [SerializeField] private CollectableTypeSO _collectable;
    [SerializeField] private AchievementInfoSO _previousAchievement;
    [SerializeField] private CollectableTypeListSO _collectableList;
    [SerializeField] private AchievementListSO _achievementListReference;
    [SerializeField] private bool _manualGoalAmount;
    [SerializeField] private bool _requiresPreviousAchievement;
    [SerializeField] private bool _showProgression;
    [SerializeField] private bool _isHidden;
    [SerializeField] private bool _unlocked;
    [SerializeField] private EventReference _soundEffect;

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
    public CompletionEnumType CompletionType
    {
        get
        {
            return _completionType;
        }
    }
    public CollectableEnumType CollectableType
    {
        get
        {
            return _collectableType;
        }
    }
    public CollectableRequirementEnumType CollectableRequirementType
    {
        get
        {
            return _collectableRequirementType;
        }
    }
    public CollectableTypeSO Collectable
    {
        get
        {
            return _collectable;
        }
    }
    public AchievementInfoSO PreviousAchievement
    {
        get
        {
            return _previousAchievement;
        }
    }
    public List<BaseCollectableTypeSO> CollectableList
    {
        get
        {
            if(_collectableList != null)
            {
                return _collectableList.CollectablesList;
            }
            return null;
        }
    }
    public AchievementListSO Achievements
    {
        get
        {
            return _achievementListReference;
        }
    }
    public int AchievementCount
    {
        get
        {
            int total = 0;
            foreach(AchievementInfoSO achievement in _achievementListReference.AchievementList)
            {
                if(achievement.CollectableType != CollectableEnumType.Achievement)
                {
                    total++;
                }
            }
            return total;
        }
    }
    public int CurrentIntAmount
    {
        get
        {
            return _intCurrentAmount;
        }
    }
    public int IntGoal
    {
        get
        {
            return _intGoalAmount;
        }
    }
    public float CurrentFloatAmount
    {
        get
        {
            return _floatCurrentAmount;
        }
    }
    public float FloatGoal
    {
        get
        {
            return _floatGoalAmount;
        }
    }
    public bool ManualGoalAmount
    {
        get
        {
            return _manualGoalAmount;
        }
    }
    public bool RequiresPreviousAchievement
    {
        get
        {
            return _requiresPreviousAchievement;
        }
    }
    public bool ShowProgression
    {
        get
        {
            return _showProgression;
        }
    }
    public bool IsHidden
    {
        get
        {
            return _isHidden;
        }
    }
    public bool IsUnlocked
    {
        get
        {
            return _unlocked;
        }
    }
    public bool AchievementUnlocked
    {
        set
        {
            _unlocked = value;
        }
    }
    public EventReference SoundEffect
    {
        get
        {
            return _soundEffect;
        }
    }
    public void UpdateCurrentAmount(int? newIntAmount, float? newFloatAmount)
    {
        if(newIntAmount != null)
        {
            _intCurrentAmount = (int)newIntAmount;
        }
        else if(newFloatAmount != null)
        {
            _floatCurrentAmount = (float)newFloatAmount;
        }
    }
}
