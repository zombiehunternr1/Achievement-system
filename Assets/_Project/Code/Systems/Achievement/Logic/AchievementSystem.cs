using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementSystem : MonoBehaviour
{
    [Header("Achievement List Reference")]
    [SerializeField] private AchievementTypeList _allAchievementsListReference;

    [Header("Event Channels")]
    [SerializeField] private EventChannel _setupAchievementUI;
    [SerializeField] private EventChannel _updateAchievementUIStatus;
    [SerializeField] private EventChannel _achievementUnlockedUI;
    [SerializeField] private EventChannel _updateProgression;
    [SerializeField] private EventChannel _saveGame;

    private void Start()
    {
        EventDispatcher.Raise(_setupAchievementUI, _allAchievementsListReference.AllAchievements);
    }

    public void ResetAllAchievements()
    {
        List<AchievementType> allAchievements = _allAchievementsListReference.AllAchievements;

        for (int i = 0; i < allAchievements.Count; i++)
        {
            AchievementType achievement = allAchievements[i];
            achievement.LockAchievement();
            achievement.SetCurrentValue(0);
            StartCoroutine(DelayUpdateUnlockedStatus(achievement));
        }

        EventDispatcher.Raise(_saveGame);
    }

    public void CheckCollectableRequest(EventPayload payload)
    {
        CollectableItem collectable = EventReader.Get<CollectableItem>(payload);
        List<AchievementType> allAchievements = _allAchievementsListReference.AllAchievements;
        List<AchievementType> triggeredAchievements = new List<AchievementType>();

        for (int i = 0; i < allAchievements.Count; i++)
        {
            AchievementType achievement = allAchievements[i];

            if (achievement.IsUnlocked || achievement.CompletionEnumRequirement != CompletionRequirementType.CollectableRequirement)
            {
                continue;
            }

            if (!achievement.IsAchievementRelated(collectable))
            {
                continue;
            }

            EventDispatcher.Raise(_updateAchievementUIStatus, achievement);

            if (achievement.IsCollectableGoalReached(collectable))
            {
                triggeredAchievements.Add(achievement);
            }
        }

        Dictionary<AchievementType, List<AchievementType>> dependencyGraph = new Dictionary<AchievementType, List<AchievementType>>();

        for (int i = 0; i < triggeredAchievements.Count; i++)
        {
            AchievementType achievement = triggeredAchievements[i];

            if (!dependencyGraph.ContainsKey(achievement))
            {
                dependencyGraph[achievement] = new List<AchievementType>();
            }

            for (int j = 0; j < allAchievements.Count; j++)
            {
                AchievementType maybeDependent = allAchievements[j];

                if (!maybeDependent.IsUnlockedAfterAchievement || maybeDependent.IsUnlocked)
                {
                    continue;
                }

                if (maybeDependent.UnlockAfterAchievements.Contains(achievement))
                {
                    if (!dependencyGraph.ContainsKey(maybeDependent))
                    {
                        dependencyGraph[maybeDependent] = new List<AchievementType>();
                    }

                    if (!dependencyGraph[achievement].Contains(maybeDependent))
                    {
                        dependencyGraph[achievement].Add(maybeDependent);
                    }
                }
            }
        }

        List<AchievementType> sorted = TopologicalSort(dependencyGraph);

        for (int i = 0; i < sorted.Count; i++)
        {
            AchievementType achievement = sorted[i];

            if (IsEligibleForUnlock(achievement, collectable))
            {
                UnlockAchievement(achievement);
            }
        }
    }

    public void UpdateReceivedAchievement(EventPayload payload)
    {
        AchievementTrigger trigger = EventReader.Get<AchievementTrigger>(payload);
        AchievementType achievement = FindAchievementById(trigger.Id);

        if (achievement == null)
        {
            Debug.LogWarning("Couldn't find achievement with ID: " + trigger.Id);
            return;
        }

        if (IsEligibleForUnlock(achievement, trigger.Value))
        {
            UnlockAchievement(achievement);
        }
        else
        {
            EventDispatcher.Raise(_updateAchievementUIStatus, achievement);
        }
    }

    private void UnlockAchievement(AchievementType achievement)
    {
        if (achievement.IsUnlocked)
        {
            return;
        }

        achievement.UnlockAchievement();
        EventDispatcher.Raise(_saveGame);
        EventDispatcher.Raise(_updateAchievementUIStatus, achievement);
        EventDispatcher.Raise(_achievementUnlockedUI, achievement);
        CheckPendingAchievementUnlocks();
    }

    private void CheckPendingAchievementUnlocks()
    {
        List<AchievementType> allAchievements = _allAchievementsListReference.AllAchievements;

        for (int i = 0; i < allAchievements.Count; i++)
        {
            AchievementType achievement = allAchievements[i];

            if (achievement.CompletionEnumRequirement != CompletionRequirementType.AchievementRequirement || achievement.IsUnlocked)
            {
                continue;
            }

            if (achievement.IsAchievementGoalReached)
            {
                UnlockAchievement(achievement);
                return;
            }

            EventDispatcher.Raise(_updateAchievementUIStatus, achievement);
        }
    }

    #region Coroutines

    private IEnumerator DelayUpdateUnlockedStatus(AchievementType achievement)
    {
        yield return new WaitForSeconds(0.01f);
        EventDispatcher.Raise(_updateAchievementUIStatus, achievement);
    }

    #endregion

    #region Saving & Loading

    public void UpdateData(EventPayload payload)
    {
        GameData gameData = EventReader.Get<GameData>(payload);
        bool isLoading = EventReader.Get<bool>(payload);

        if (isLoading)
        {
            LoadAchievementDataFromGameData(gameData);
        }
        else
        {
            SaveAchievementDataToGameData(gameData);
        }

        EventDispatcher.Raise(_updateProgression, gameData);
    }

    private void LoadAchievementDataFromGameData(GameData gameData)
    {
        List<AchievementType> allAchievements = _allAchievementsListReference.AllAchievements;

        for (int i = 0; i < allAchievements.Count; i++)
        {
            AchievementType achievement = allAchievements[i];
            gameData.AchievementsData.TryGetValue(achievement.AchievementId, out AchievementDTO achievementDTO);
            achievement.LoadAchievementStatus(achievementDTO);
            EventDispatcher.Raise(_updateAchievementUIStatus, achievement);
        }
    }

    private void SaveAchievementDataToGameData(GameData gameData)
    {
        foreach (AchievementType achievement in _allAchievementsListReference.AllAchievements)
        {
            achievement.SaveAchievementStatus(gameData);
        }
    }

    #endregion

    #region Helpers

    private AchievementType FindAchievementById(string achievementID)
    {
        List<AchievementType> allAchievements = _allAchievementsListReference.AllAchievements;

        for (int i = 0; i < allAchievements.Count; i++)
        {
            AchievementType achievement = allAchievements[i];

            if (achievement != null && achievement.AchievementId == achievementID)
            {
                return achievement;
            }
        }

        return null;
    }

    private List<AchievementType> TopologicalSort(Dictionary<AchievementType, List<AchievementType>> dependencyGraph)
    {
        List<AchievementType> sortedList = new List<AchievementType>();
        HashSet<AchievementType> visited = new HashSet<AchievementType>();

        void Visit(AchievementType node)
        {
            if (visited.Contains(node))
            {
                return;
            }

            visited.Add(node);

            List<AchievementType> neighbors = dependencyGraph[node];
            for (int i = 0; i < neighbors.Count; i++)
            {
                Visit(neighbors[i]);
            }

            sortedList.Add(node);
        }

        foreach (AchievementType achievement in dependencyGraph.Keys)
        {
            Visit(achievement);
        }

        sortedList.Reverse();
        return sortedList;
    }

    private bool IsEligibleForUnlock(AchievementType achievement, object context = null)
    {
        if (achievement.IsUnlocked)
        {
            return false;
        }

        switch (achievement.CompletionEnumRequirement)
        {
            case CompletionRequirementType.NoRequirement: return true;
            case CompletionRequirementType.ValueRequirement: achievement.SetCurrentValue(context); return achievement.IsValueGoalReached;
            case CompletionRequirementType.CollectableRequirement: return achievement.IsCollectableGoalReached((CollectableItem)context);
            case CompletionRequirementType.AchievementRequirement: return achievement.IsAchievementGoalReached;
        }

        return false;
    }

    #endregion
}