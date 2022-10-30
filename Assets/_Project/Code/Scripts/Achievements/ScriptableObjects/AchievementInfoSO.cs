using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "Achievement", menuName = "Scriptable Objects/Achievements/Achievement")]
public class AchievementInfoSO : ScriptableObject
{
    [UniqueIdentifier]
    [SerializeField] private string _achievementId;
    [SerializeField] private string _title;
    [SerializeField] private string _description;
    [SerializeField] private Sprite _icon;
    public enum completionType { noRequirements, integerRequirement, floatRequirement };
    [SerializeField] private completionType _CompletionType;
    public enum CollectableType { None, Collectable };
    [SerializeField] private CollectableType _CollectableType;

    [SerializeField] private int _intCurrentAmount;
    [SerializeField] private int _intGoalAmount;
    [SerializeField] private float _floatCurrentAmount;
    [SerializeField] private float _floatGoalAmount;
    [SerializeField] private CollectableTypeList _collectable;
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
    public CollectableType collectableType
    {
        get
        {
            return _CollectableType;
        }
    }
    public CollectableTypeList Collectable
    {
        get
        {
            return _collectable;
        }
    }
    public int CurrentIntAmount
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
        set
        {
            _floatCurrentAmount = value;
        }
    }
    public float FloatGoal
    {
        get
        {
            return _floatGoalAmount;
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
}
