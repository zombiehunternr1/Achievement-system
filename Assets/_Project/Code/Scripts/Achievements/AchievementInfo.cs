using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "Achievement", menuName = "Scriptable Objects/Achievement")]
public class AchievementInfo : ScriptableObject
{
    [SerializeField] private int achievementID;
    [SerializeField] private string title;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    public enum completionType { noRequirements, integerRequirement, floatRequirement };
    [SerializeField]
    private completionType valueCompletionType;

    [SerializeField] private int intGoalAmount;
    [SerializeField] private float floatGoalAmount;
    [SerializeField] private bool isHidden;
    [SerializeField] private bool unlocked;
    [SerializeField] private EventReference soundEffect;

    public int GetAchievementID()
    {
        return achievementID;
    }

    public string GetTitle()
    {
        return title;
    }

    public string GetDescription()
    {
        return description;
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public int GetIntGoal()
    {
        return intGoalAmount;
    }

    public float GetFloatGoal()
    {
        return floatGoalAmount;
    }

    public bool CheckIfHidden()
    {
        return isHidden;
    }

    public bool CheckIfUnlocked()
    {
        return unlocked;
    }

    public bool UnlockAchievement()
    {
        return unlocked = true;
    }

    public EventReference GetSoundEffect()
    {
        return soundEffect;
    }
}
