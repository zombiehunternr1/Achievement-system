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
        for (int i = 0; i < _allAchievementsListReference.AllAchievements.Count; i++)
        {
            if (_allAchievementsListReference.AllAchievements[i] != null &&
                _allAchievementsListReference.AllAchievements[i].AchievementId == achievementID)
            {
                return _allAchievementsListReference.AllAchievements[i];
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
                foreach (AchievementType neighbor in dependencyGraph[achievement])
                {
                    Visit(neighbor);
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
    private bool ShouldSkipAchievement(AchievementType achievement, CollectableItem collectable)
    {
        return achievement.IsUnlocked
            || achievement.CompletionEnumRequirement != CompletionRequirementType.CollectableRequirement
            || !achievement.IsAchievementRelated(collectable);
    }
    private void Start()
    {
        EventPackageFactory.BuildAndInvoke(_setupAchievementUI, _allAchievementsListReference.AllAchievements);
    }
    public void ResetAllAchievements()
    {
        foreach (AchievementType achievement in _allAchievementsListReference.AllAchievements)
        {
            achievement.LockAchievement();
            achievement.SetCurrentValue(0);
            StartCoroutine(DelayUpdateUnlockedStatus(achievement));
        }
        EventPackageFactory.BuildAndInvoke(_saveGame);
    }
    public void CheckCollectableRequest(EventData eventData)
    {
        CollectableItem collectable = EventPackageExtractor.ExtractEventData<CollectableItem>(eventData);
        Dictionary<AchievementType, List<AchievementType>> dependencyGraph = new Dictionary<AchievementType, List<AchievementType>>();

        foreach (AchievementType achievement in _allAchievementsListReference.AllAchievements)
        {
            if (ShouldSkipAchievement(achievement, collectable))
            {
                continue;
            }
            EventPackageFactory.BuildAndInvoke(_updateAchievementUIStatus, achievement);
            if (!achievement.IsCollectableGoalReached(collectable))
            {
                continue;
            }
            if (!dependencyGraph.ContainsKey(achievement))
            {
                dependencyGraph[achievement] = new List<AchievementType>();
            }
        }

        List<AchievementType> sortedAchievements = TopologicalSort(dependencyGraph);
        foreach (AchievementType achievement in sortedAchievements)
        {
            UnlockAchievement(achievement);
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
            achievement.CompletionEnumRequirement == CompletionRequirementType.AchievementRequirement && achievement.IsAchievementGoalReached)
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
    private void CheckAchievementTypes()
    {
        for (int i =0; i < _allAchievementsListReference.AllAchievements.Count; i++)
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
    #region Co-routines
    private IEnumerator DelayUpdateUnlockedStatus(AchievementType achievement)
    {
        yield return new WaitForSeconds(0.01f);
        EventPackageFactory.BuildAndInvoke(_updateAchievementUIStatus, achievement);
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
        EventPackageFactory.BuildAndInvoke(_updateProgression, gameData);
    }
    private void LoadAchievementDataFromGameData(GameData gameData)
    {
        foreach (AchievementType achievement in _allAchievementsListReference.AllAchievements)
        {
            gameData.AchievementsData.TryGetValue(achievement.AchievementId, out AchievementDTO achievementDTO);
            achievement.LoadAchievementStatus(achievementDTO);
            EventPackageFactory.BuildAndInvoke(_updateAchievementUIStatus, achievement);
        }
    }
    private void SaveAchievementDataToGameData(GameData gameData)
    {
        List<AchievementType>.Enumerator enumAchievementsList = _allAchievementsListReference.AllAchievements.GetEnumerator();
        try
        {
            while (enumAchievementsList.MoveNext())
            {
                enumAchievementsList.Current.SaveAchievementStatus(gameData);
            }
        }
        finally
        {
            enumAchievementsList.Dispose();
        }
    }
    #endregion
}