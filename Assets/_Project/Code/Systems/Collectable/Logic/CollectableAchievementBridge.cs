using System.Collections.Generic;
using UnityEngine;

public class CollectableAchievementBridge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AchievementSystem _achievementSystem;

    // Called via EventListenerGroup when _updateProgression fires.
    // _updateProgression already exists and fires after every load — no new
    // event channel needed. This corrects collectable progression displays
    // that the core system left empty during the load pass.
    public void RefreshProgressionDisplays(EventPayload payload)
    {
        List<AchievementType> allAchievements = _achievementSystem.GetAllAchievements();

        for (int i = 0; i < allAchievements.Count; i++)
        {
            AchievementType achievement = allAchievements[i];

            if (achievement.CompletionEnumRequirement != CompletionRequirementType.CollectableRequirement)
            {
                continue;
            }

            if (achievement.CollectableRequirementData == null)
            {
                continue;
            }

            string progressionDisplay = GetProgressionString(achievement);
            _achievementSystem.RaiseUIStatus(achievement, progressionDisplay);
        }
    }

    // Called via EventListenerGroup when a collectable is collected.
    public void CheckCollectableRequest(EventPayload payload)
    {
        CollectableItem collectable = EventReader.Get<CollectableItem>(payload);
        List<AchievementType> allAchievements = _achievementSystem.GetAllAchievements();
        List<AchievementType> triggeredAchievements = new List<AchievementType>();

        for (int i = 0; i < allAchievements.Count; i++)
        {
            AchievementType achievement = allAchievements[i];

            if (_achievementSystem.IsUnlocked(achievement.AchievementId) ||
                achievement.CompletionEnumRequirement != CompletionRequirementType.CollectableRequirement)
            {
                continue;
            }

            if (achievement.CollectableRequirementData == null)
            {
                continue;
            }

            if (!achievement.CollectableRequirementData.IsRelatedToAchievement(collectable))
            {
                continue;
            }

            string progressionDisplay = GetProgressionString(achievement);
            _achievementSystem.RaiseUIStatus(achievement, progressionDisplay);

            if (achievement.CollectableRequirementData.IsRequirementMet(collectable))
            {
                triggeredAchievements.Add(achievement);
            }
        }

        _achievementSystem.ProcessTriggeredAchievements(triggeredAchievements);
    }

    private string GetProgressionString(AchievementType achievement)
    {
        switch (achievement.CollectableRequirementData.CollectableRequirement)
        {
            case CollectableRequirementType.SingleCollectable:
                {
                    (int currentAmount, int totalAmount) = achievement.CollectableRequirementData.GetSingleProgressionDisplay();
                    return achievement.FormatProgressionDisplay(currentAmount, totalAmount);
                }
            case CollectableRequirementType.AllCollectables:
                {
                    (int currentAmount, int totalAmount) = achievement.CollectableRequirementData.GetAllProgressionDisplay();
                    return achievement.FormatProgressionDisplay(currentAmount, totalAmount);
                }
            default:
                {
                    (int currentAmount, int totalAmount) = achievement.CollectableRequirementData.GetCustomAmountDisplay();
                    return achievement.FormatProgressionDisplay(currentAmount, totalAmount);
                }
        }
    }
}