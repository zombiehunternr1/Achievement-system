using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "Scriptable Objects/Achievement")]
public class AchievementInfo : ScriptableObject
{
    public string title;
    public string description;
    public Sprite icon;
    public enum completionType { noRequirements, integerRequirement, floatRequirement};
    public completionType valueCompletionType;

    public int intGoalAmount;
    public float floatGoalAmount;

    public bool isHidden;
    public bool unlocked;
}
