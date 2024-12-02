using UnityEngine;
using System;

[Serializable]
public class RequirementData
{
    [SerializeField] private bool _requiresPreviousAchievementToUnlock;
    [SerializeField] private AchievementSO _previousAchievementReference;
    [SerializeField] private CompletionEnumRequirement _completionEnumRequirement;
    public bool RequiresPreviousAchievementToUnlock
    {
        get
        {
            return _requiresPreviousAchievementToUnlock;
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
