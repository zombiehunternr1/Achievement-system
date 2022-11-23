using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "Scriptable Objects/Achievements/Achievement")]
public class AchievementInfoSO : ScriptableObject
{
    [UniqueIdentifier]
    [SerializeField] private string _achievementId;
    [SerializeField] private string _title;
    [SerializeField] private string _description;
    [SerializeField] private Sprite _icon;
    public enum CompletionType { noRequirements, integerRequirement, floatRequirement };
    [SerializeField] private CompletionType _completionType;
    public enum CollectableType { none, collectable, achievement };
    [SerializeField] private CollectableType _collectableType;
    public enum CollectableRequirementType { single, list};
    [SerializeField] private CollectableRequirementType _collectableRequirementType;

    [SerializeField] private int _intCurrentAmount;
    [SerializeField] private int _intGoalAmount;
    [SerializeField] private float _floatCurrentAmount;
    [SerializeField] private float _floatGoalAmount;
    [SerializeField] private CollectableTypeSO _collectable;
    [SerializeField] private AchievementInfoSO _previousAchievement;
    [SerializeField] private CollectableTypeListSO _collectableList;
    [SerializeField] private AchievementListSO _achievementList;
    [SerializeField] private bool _manualGoalAmount;
    [SerializeField] private bool _requiresPreviousAchievement;
    [SerializeField] private bool _showProgression;
    [SerializeField] private bool _isHidden;
    [SerializeField] private bool _unlocked;
    [SerializeField] private EventReference _soundEffect;

    public string achievementId
    {
        get
        {
            return _achievementId;
        }
    }
    public string title
    {
        get
        {
            return _title;
        }
    }
    public string description
    {
        get
        {
            return _description;
        }
    }
    public Sprite icon
    {
        get
        {
            return _icon;
        }
    }
    public CollectableType collectableType
    {
        get
        {
            return _collectableType;
        }
    }
    public CollectableRequirementType collectableRequirementType
    {
        get
        {
            return _collectableRequirementType;
        }
    }
    public CollectableTypeSO collectable
    {
        get
        {
            return _collectable;
        }
    }
    public AchievementInfoSO previousAchievement
    {
        get
        {
            return _previousAchievement;
        }
    }
    public CollectableTypeListSO collectableList
    {
        get
        {
            return _collectableList;
        }
    }
    public AchievementListSO achievements
    {
        get
        {
            return _achievementList;
        }
    }
    public int achievementCount
    {
        get
        {
            int total = 0;
            foreach(AchievementInfoSO achievement in _achievementList.achievementList)
            {
                if(achievement.collectableType != CollectableType.achievement)
                {
                    total++;
                }
            }
            return total;
        }
    }
    public int currentIntAmount
    {
        get
        {
            return _intCurrentAmount;
        }
        set
        {
            _intCurrentAmount = value;
        }
    }
    public int intGoal
    {
        get
        {
            return _intGoalAmount;
        }
    }
    public float currentFloatAmount
    {
        get
        {
            return _floatCurrentAmount;
        }
        set
        {
            _floatCurrentAmount = value;
        }
    }
    public float floatGoal
    {
        get
        {
            return _floatGoalAmount;
        }
    }
    public bool manualGoalAmount
    {
        get
        {
            return _manualGoalAmount;
        }
    }
    public bool requiresPreviousAchievement
    {
        get
        {
            return _requiresPreviousAchievement;
        }
    }
    public bool showProgression
    {
        get
        {
            return _showProgression;
        }
    }
    public bool isHidden
    {
        get
        {
            return _isHidden;
        }
    }
    public bool isUnlocked
    {
        get
        {
            return _unlocked;
        }
    }
    public bool achievementUnlocked
    {
        set
        {
            _unlocked = value;
        }
    }
    public EventReference soundEffect
    {
        get
        {
            return _soundEffect;
        }
    }
}
