using System;
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

    // Runtime unlock state — no longer stored in ScriptableObjects
    private readonly Dictionary<string, bool> _unlockedState = new Dictionary<string, bool>();

    public bool IsUnlocked(string achievementId)
        => _unlockedState.TryGetValue(achievementId, out bool value) && value;

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
            _unlockedState[achievement.AchievementId] = false;
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

            if (IsUnlocked(achievement.AchievementId) ||
                achievement.CompletionEnumRequirement != CompletionRequirementType.CollectableRequirement)
            {
                continue;
            }

            if (!achievement.IsAchievementRelated(collectable))
            {
                continue;
            }

            RaiseUIStatus(achievement);

            if (achievement.IsCollectableGoalReached(collectable))
            {
                triggeredAchievements.Add(achievement);
            }
        }

        Dictionary<AchievementType, List<AchievementType>> dependencyGraph =
            new Dictionary<AchievementType, List<AchievementType>>();

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

                if (!maybeDependent.IsUnlockedAfterAchievement || IsUnlocked(maybeDependent.AchievementId))
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
            RaiseUIStatus(achievement);
        }
    }

    private void UnlockAchievement(AchievementType achievement)
    {
        if (IsUnlocked(achievement.AchievementId))
        {
            return;
        }

        _unlockedState[achievement.AchievementId] = true;

        EventDispatcher.Raise(_saveGame);
        RaiseUIStatus(achievement);
        EventDispatcher.Raise(_achievementUnlockedUI,
            new AchievementStatusPayload(achievement, true,
                achievement.GetProgressionDisplay(IsUnlocked)));

        CheckPendingAchievementUnlocks();
    }

    private void CheckPendingAchievementUnlocks()
    {
        List<AchievementType> allAchievements = _allAchievementsListReference.AllAchievements;

        for (int i = 0; i < allAchievements.Count; i++)
        {
            AchievementType achievement = allAchievements[i];

            if (achievement.CompletionEnumRequirement != CompletionRequirementType.AchievementRequirement ||
                IsUnlocked(achievement.AchievementId))
            {
                continue;
            }

            if (achievement.IsAchievementGoalReached(IsUnlocked))
            {
                UnlockAchievement(achievement);
                return;
            }

            RaiseUIStatus(achievement);
        }
    }

    private void RaiseUIStatus(AchievementType achievement)
    {
        EventDispatcher.Raise(_updateAchievementUIStatus,
            new AchievementStatusPayload(
                achievement,
                IsUnlocked(achievement.AchievementId),
                achievement.GetProgressionDisplay(IsUnlocked)));
    }

    #region Coroutines

    private IEnumerator DelayUpdateUnlockedStatus(AchievementType achievement)
    {
        yield return new WaitForSeconds(0.01f);
        RaiseUIStatus(achievement);
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
            gameData.AchievementsData.TryGetValue(achievement.AchievementId, out AchievementDTO dto);

            // Restore unlock state into runtime dictionary
            _unlockedState[achievement.AchievementId] = dto != null && dto.IsUnlocked;

            // Restore value progress into the SO's ValueData (unchanged)
            achievement.LoadAchievementStatus(dto);

            RaiseUIStatus(achievement);
        }
    }

    private void SaveAchievementDataToGameData(GameData gameData)
    {
        foreach (AchievementType achievement in _allAchievementsListReference.AllAchievements)
        {
            achievement.SaveAchievementStatus(gameData, IsUnlocked(achievement.AchievementId));
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

    private List<AchievementType> TopologicalSort(
        Dictionary<AchievementType, List<AchievementType>> dependencyGraph)
    {
        List<AchievementType> sortedList = new List<AchievementType>();
        HashSet<AchievementType> visited = new HashSet<AchievementType>();

        void Visit(AchievementType node)
        {
            if (visited.Contains(node)) return;

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
        if (IsUnlocked(achievement.AchievementId))
        {
            return false;
        }

        switch (achievement.CompletionEnumRequirement)
        {
            case CompletionRequirementType.NoRequirement:
                return true;
            case CompletionRequirementType.ValueRequirement:
                achievement.SetCurrentValue(context);
                return achievement.IsValueGoalReached;
            case CompletionRequirementType.CollectableRequirement:
                return achievement.IsCollectableGoalReached((CollectableItem)context);
            case CompletionRequirementType.AchievementRequirement:
                return achievement.IsAchievementGoalReached(IsUnlocked);
        }

        return false;
    }

    #endregion
}