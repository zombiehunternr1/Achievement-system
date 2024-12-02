using UnityEngine;
using System;

[Serializable]
public class RequirementData
{
    [SerializeField] private bool _requiresPreviousAchievement;
    [SerializeField] private AchievementSO _previousAchievementReference;
    [SerializeField] private CompletionEnumRequirement _completionEnumRequirement;
    public bool RequiresPreviousAchievement
    {
        get
        {
            return _requiresPreviousAchievement;
        }
    }
    public AchievementSO PreviousAchievementReference
    {
        get
        {
            return _previousAchievementReference;
        }
    }
    public CompletionEnumRequirement CompletionEnumRequirement
    {
        get
        {
            return _completionEnumRequirement;
        }
    }
}
