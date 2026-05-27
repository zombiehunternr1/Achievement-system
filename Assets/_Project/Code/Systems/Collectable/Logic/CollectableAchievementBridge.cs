using System.Collections.Generic;
using UnityEngine;

public class CollectableAchievementBridge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AchievementSystem _achievementSystem;

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

            if (!achievement.CollectableRequirementData.IsRelatedToAchievement(collectable))
            {
                continue;
            }

            _achievementSystem.RaiseUIStatus(achievement);

            if (achievement.CollectableRequirementData.IsRequirementMet(collectable))
            {
                triggeredAchievements.Add(achievement);
            }
        }

        _achievementSystem.ProcessTriggeredAchievements(triggeredAchievements);
    }
}