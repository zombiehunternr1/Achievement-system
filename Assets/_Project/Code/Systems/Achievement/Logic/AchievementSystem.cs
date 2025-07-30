using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementSystem : MonoBehaviour
{
    [Header("Achievement list reference")]
    [SerializeField] private AchievementTypeList _allAchievementsListReference;

    [Header("Event references")]
    [SerializeField] private EventPackage _setupAchievementUI;
    [SerializeField] private EventPackage _updateAchievementUIStatus;
    [SerializeField] private EventPackage _achievementUnlockedUI;
    [SerializeField] private EventPackage _updateProgression;
    [SerializeField] private EventPackage _saveGame;
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

        void Visit(AchievementType achievement)
        {
            if (!visited.Contains(achievement))
            {
                visited.Add(achievement);

                List<AchievementType> neighbors = dependencyGraph[achievement];

                for (int i = 0; i < neighbors.Count; i++)
                {
                    Visit(neighbors[i]);
                }

                sortedList.Add(achievement);
            }
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
            case CompletionRequirementType.NoRequirement:
                {
                    return true;
                }
            case CompletionRequirementType.ValueRequirement:
                {
                    achievement.SetCurrentValue(context);
                    return achievement.IsValueGoalReached;
                }
            case CompletionRequirementType.CollectableRequirement:
                {
                    return achievement.IsCollectableGoalReached((CollectableItem)context);
                }
            case CompletionRequirementType.AchievementRequirement:
                {
                    return achievement.IsAchievementGoalReached;
                }
        }

        return false;
    }
    private void Start()
    {
        ExecuteEventPackage(_setupAchievementUI, _allAchievementsListReference.AllAchievements);
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

        ExecuteEventPackage(_saveGame);
    }

    public void CheckCollectableRequest(EventData eventData)
    {
        CollectableItem collectable = EventPackageExtractor.ExtractEventData<CollectableItem>(eventData);
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

            ExecuteEventPackage(_updateAchievementUIStatus, achievement);

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

    public void UpdateRecievedAchievement(EventData eventData)
    {
        string achievementID = EventPackageExtractor.ExtractEventData<string>(eventData);
        object valueObj = null;

        if (EventPackageExtractor.ContainsData(eventData))
        {
            valueObj = EventPackageExtractor.ExtractAdditionalData(eventData);
        }

        AchievementType achievement = FindAchievementById(achievementID);

        if (achievement == null)
        {
            Debug.LogWarning("Couldn't find the achievement in the list with ID: " + achievementID);
            return;
        }

        if (IsEligibleForUnlock(achievement, valueObj))
        {
            UnlockAchievement(achievement);
        }
        else
        {
            ExecuteEventPackage(_updateAchievementUIStatus, achievement);
        }
    }
    private void UnlockAchievement(AchievementType achievement)
    {
        if (achievement.IsUnlocked)
        {
            return;
        }

        achievement.UnlockAchievement();
        ExecuteEventPackage(_saveGame);
        ExecuteEventPackage(_updateAchievementUIStatus, achievement);
        ExecuteEventPackage(_achievementUnlockedUI, achievement);
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

            ExecuteEventPackage(_updateAchievementUIStatus, achievement);
        }
    }
    private void ExecuteEventPackage(EventPackage package, object arg = null)
    {
        EventPackageFactory.BuildAndInvoke(package, arg);
    }

    #region Co-routines
    private IEnumerator DelayUpdateUnlockedStatus(AchievementType achievement)
    {
        yield return new WaitForSeconds(0.01f);
        ExecuteEventPackage(_updateAchievementUIStatus, achievement);
    }
    #endregion

    #region Saving & Loading
    public void UpdateData(EventData eventData)
    {
        GameData gameData = EventPackageExtractor.ExtractEventData<GameData>(eventData);
        bool isLoading = EventPackageExtractor.ExtractEventData<bool>(eventData);

        if (isLoading)
        {
            LoadAchievementDataFromGameData(gameData);
        }
        else
        {
            SaveAchievementDataToGameData(gameData);
        }

        ExecuteEventPackage(_updateProgression, gameData);
    }

    private void LoadAchievementDataFromGameData(GameData gameData)
    {
        List<AchievementType> allAchievements = _allAchievementsListReference.AllAchievements;

        for (int i = 0; i < allAchievements.Count; i++)
        {
            AchievementType achievement = allAchievements[i];
            gameData.AchievementsData.TryGetValue(achievement.AchievementId, out AchievementDTO achievementDTO);
            achievement.LoadAchievementStatus(achievementDTO);
            ExecuteEventPackage(_updateAchievementUIStatus, achievement);
        }
    }

    private void SaveAchievementDataToGameData(GameData gameData)
    {
        List<AchievementType>.Enumerator enumerator = _allAchievementsListReference.AllAchievements.GetEnumerator();

        try
        {
            while (enumerator.MoveNext())
            {
                enumerator.Current.SaveAchievementStatus(gameData);
            }
        }
        finally
        {
            enumerator.Dispose();
        }
    }
    #endregion
}