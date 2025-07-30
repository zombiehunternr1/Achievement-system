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

    private void Start()
    {
        EventPackageFactory.BuildAndInvoke(_setupAchievementUI, _allAchievementsListReference.AllAchievements);
    }

    public void ResetAllAchievements()
    {
        for (int i = 0; i < _allAchievementsListReference.AllAchievements.Count; i++)
        {
            AchievementType achievement = _allAchievementsListReference.AllAchievements[i];
            achievement.LockAchievement();
            achievement.SetCurrentValue(0);
            StartCoroutine(DelayUpdateUnlockedStatus(achievement));
        }

        EventPackageFactory.BuildAndInvoke(_saveGame);
    }

    public void CheckCollectableRequest(EventData eventData)
    {
        CollectableItem collectable = EventPackageExtractor.ExtractEventData<CollectableItem>(eventData);

        List<AchievementType> triggeredAchievements = new List<AchievementType>();

        for (int i = 0; i < _allAchievementsListReference.AllAchievements.Count; i++)
        {
            AchievementType achievement = _allAchievementsListReference.AllAchievements[i];

            if (achievement.IsUnlocked || achievement.CompletionEnumRequirement != CompletionRequirementType.CollectableRequirement || !achievement.IsAchievementRelated(collectable))
            {
                continue;
            }

            EventPackageFactory.BuildAndInvoke(_updateAchievementUIStatus, achievement);

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

            for (int j = 0; j < _allAchievementsListReference.AllAchievements.Count; j++)
            {
                AchievementType maybeDependent = _allAchievementsListReference.AllAchievements[j];

                if (!maybeDependent.IsUnlockedAfterAchievement || maybeDependent.IsUnlocked)
                {
                    continue;
                }

                for (int k = 0; k < maybeDependent.UnlockAfterAchievements.Count; k++)
                {
                    if (maybeDependent.UnlockAfterAchievements[k] == achievement)
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
        }

        List<AchievementType> sorted = TopologicalSort(dependencyGraph);

        for (int i = 0; i < sorted.Count; i++)
        {
            AchievementType achievement = sorted[i];

            if (IsRequirementMet(achievement, collectable))
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

        CheckAchievementRequirementStatus(achievement, valueObj);
    }

    private void CheckAchievementRequirementStatus(AchievementType achievement, object valueObj)
    {
        if (achievement.IsUnlocked)
        {
            return;
        }

        if (achievement.CompletionEnumRequirement == CompletionRequirementType.NoRequirement ||
            (achievement.CompletionEnumRequirement == CompletionRequirementType.AchievementRequirement && achievement.IsAchievementGoalReached))
        {
            UnlockAchievement(achievement);
            return;
        }

        if (achievement.CompletionEnumRequirement == CompletionRequirementType.ValueRequirement)
        {
            HandleValueUpdate(achievement, valueObj);
            return;
        }
    }

    private void HandleValueUpdate(AchievementType achievement, object valueObj)
    {
        achievement.SetCurrentValue(valueObj);

        if (achievement.IsValueGoalReached)
        {
            UnlockAchievement(achievement);
        }
        else
        {
            EventPackageFactory.BuildAndInvoke(_updateAchievementUIStatus, achievement);
        }
    }

    private void UnlockAchievement(AchievementType achievement)
    {
        if (achievement.IsUnlocked)
        {
            return;
        }

        achievement.UnlockAchievement();
        EventPackageFactory.BuildAndInvoke(_saveGame);
        EventPackageFactory.BuildAndInvoke(_updateAchievementUIStatus, achievement);
        EventPackageFactory.BuildAndInvoke(_achievementUnlockedUI, achievement);
        CheckAchievementTypes();
    }

    private bool IsRequirementMet(AchievementType achievement, CollectableItem collectable)
    {
        if (achievement.CompletionEnumRequirement == CompletionRequirementType.CollectableRequirement)
        {
            return achievement.IsCollectableGoalReached(collectable);
        }

        if (achievement.CompletionEnumRequirement == CompletionRequirementType.ValueRequirement)
        {
            return achievement.IsValueGoalReached;
        }

        if (achievement.CompletionEnumRequirement == CompletionRequirementType.AchievementRequirement)
        {
            return achievement.IsAchievementGoalReached;
        }

        return false;
    }

    private void CheckAchievementTypes()
    {
        for (int i = 0; i < _allAchievementsListReference.AllAchievements.Count; i++)
        {
            AchievementType achievement = _allAchievementsListReference.AllAchievements[i];

            if (achievement.CompletionEnumRequirement != CompletionRequirementType.AchievementRequirement || achievement.IsUnlocked)
            {
                continue;
            }

            if (achievement.IsAchievementGoalReached)
            {
                UnlockAchievement(achievement);
                return;
            }

            EventPackageFactory.BuildAndInvoke(_updateAchievementUIStatus, achievement);
        }
    }

    private AchievementType FindAchievementById(string achievementID)
    {
        for (int i = 0; i < _allAchievementsListReference.AllAchievements.Count; i++)
        {
            AchievementType achievement = _allAchievementsListReference.AllAchievements[i];

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

        foreach (AchievementType key in dependencyGraph.Keys)
        {
            Visit(key);
        }

        sortedList.Reverse();
        return sortedList;
    }

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

        EventPackageFactory.BuildAndInvoke(_updateProgression, gameData);
    }

    private void LoadAchievementDataFromGameData(GameData gameData)
    {
        for (int i = 0; i < _allAchievementsListReference.AllAchievements.Count; i++)
        {
            AchievementType achievement = _allAchievementsListReference.AllAchievements[i];
            gameData.AchievementsData.TryGetValue(achievement.AchievementId, out AchievementDTO achievementDTO);
            achievement.LoadAchievementStatus(achievementDTO);
            EventPackageFactory.BuildAndInvoke(_updateAchievementUIStatus, achievement);
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

    #region Co-routines
    private IEnumerator DelayUpdateUnlockedStatus(AchievementType achievement)
    {
        yield return new WaitForSeconds(0.01f);
        EventPackageFactory.BuildAndInvoke(_updateAchievementUIStatus, achievement);
    }
    #endregion
}
