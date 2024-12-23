using UnityEngine;
using System;

[Serializable]
public class RequirementData
{
    [SerializeField] private bool _requiresPreviousAchievementToUnlock;
    [SerializeField] private AchievementSO _previousAchievementReference;
    [SerializeField] private CompletionRequirementType _completionRequirement;
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
    public CompletionRequirementType CompletionRequirement
    {
        get
        {
            return _completionRequirement;
        }
    }
}
