using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "Achievement", menuName = "Scriptable Objects/Achievement")]
public class AchievementInfoSO : ScriptableObject
{
    [SerializeField] private int _achievementId;
    [SerializeField] private string _title;
    [SerializeField] private string _description;
    [SerializeField] private Sprite _icon;
    public enum completionType { noRequirements, integerRequirement, floatRequirement };
    [SerializeField]
    private completionType _valueCompletionType;

    [SerializeField] private int _intGoalAmount;
    [SerializeField] private float _floatGoalAmount;
    [SerializeField] private bool _isHidden;
    [SerializeField] private bool _unlocked;
    [SerializeField] private EventReference _soundEffect;

    public int AchievementId
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
    public int IntGoal
    {
        get
        {
            return _intGoalAmount;
        }
    }
    public float FloatGoal
    {
        get
        {
            return _floatGoalAmount;
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
