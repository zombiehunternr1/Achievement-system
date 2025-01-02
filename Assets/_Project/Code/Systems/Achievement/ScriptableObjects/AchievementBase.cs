using FMODUnity;
using UnityEngine;

public abstract class AchievementBase : ScriptableObject
{
    [UniqueIdentifier]
    [SerializeField] private string _achievementId;
    [SerializeField] private string _title;
    [SerializeField] private string _description;
    [SerializeField] private Sprite _icon;
    [SerializeField] private EventReference _soundEffect;
    [SerializeField] private bool _isUnlocked;
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
    public EventReference SoundEffect
    {
        get
        {
            return _soundEffect;
        }
    }
    public bool IsUnlocked
    {
        get
        {
            return _isUnlocked;
        }
    }
    public void UnlockAchievement()
    {
        _isUnlocked = true;
    }
    public void LockAchievement()
    {
        _isUnlocked = false;
    }
}
