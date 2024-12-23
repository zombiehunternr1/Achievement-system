using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AchievementSystem : MonoBehaviour
{
    [Header("Achievement list reference")]
    [SerializeField] private AchievementTypeList _allAchievementsListReference;
    [Header("Event references")]
    [SerializeField] private EventPackage _playPopUpDisplayStatus;
    [SerializeField] private EventPackage _setAchievementPopUpInfo;
    [SerializeField] private EventPackage _updateProgression;
    [SerializeField] private EventPackage _saveGame;
    [Header("Component & Settings references")]
    [SerializeField] private int _displayPopupTime = 5;
    [SerializeField] private RectTransform _achievementContainerRect;
    [SerializeField] private AchievementObject _achievementPrefabContainer;
    private List<AchievementObject> _achievementObjects = new List<AchievementObject>();
    private List<AchievementType> _queuedAchievements = new List<AchievementType>();
    private EventInstance _soundEffect;
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
    private void Awake()
    {
        SetupAchievementDisplay();
    }
    public void StartPopupCooldown()
    {
        StartCoroutine(PopupCooldown());
    }
    public void ResetAllAchievements()
    {
        if (_queuedAchievements.Count != 0)
        {
            _queuedAchievements.Clear();
        }
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
        for (int i = 0; i < _allAchievementsListReference.AllAchievements.Count; i++)
        {
            AchievementType achievement = _allAchievementsListReference.AllAchievements[i];
            if (achievement.IsUnlocked || achievement.CompletionEnumRequirement != CompletionRequirementType.CollectableRequirement)
            {
                continue;
            }
            if (!achievement.IsAchievementRelated(collectable))
            {
                continue;
            }
            UpdateAchievementStatus(achievement);
            if (!achievement.IsCollectableGoalReached(collectable))
            {
                continue;
            }
            if (!dependencyGraph.ContainsKey(achievement))
            {
                dependencyGraph[achievement] = new List<AchievementType>();
                continue;
            }
            if (achievement.RequiresPreviousAchievement && achievement.IsPreviousAchievementUnlocked)
            {
                if (!dependencyGraph.ContainsKey(achievement.PreviousAchievement))
                {
                    dependencyGraph[achievement.PreviousAchievement] = new List<AchievementType>();
                }
                continue;
            }
            dependencyGraph[achievement.PreviousAchievement].Add(achievement);
        }
        List<AchievementType> sortedAchievements = TopologicalSort(dependencyGraph);
        for (int i = 0; i < sortedAchievements.Count; i++)
        {
            UnlockAchievement(sortedAchievements[i]);
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
    private void SetupAchievementDisplay()
    {
        if (_allAchievementsListReference.AllAchievements.Count == 0)
        {
            Debug.LogWarning("The list of achievements to unlock is empty!");
            return;
        }
        for (int i = 0; i < _allAchievementsListReference.AllAchievements.Count; i++)
        {
            AchievementType achievement = _allAchievementsListReference.AllAchievements[i];
            if (achievement == null)
            {
                Debug.LogWarning("There is a missing reference at element " + i + " in the achievements to unlock list");
                continue;
            }
            AchievementObject achievementObject = Instantiate(_achievementPrefabContainer, _achievementContainerRect);
            achievementObject.SetAchievementId(achievement.AchievementId);
            _achievementObjects.Add(achievementObject);
            if (achievement.IsHidden)
            {
                achievementObject.DisableLock();
            }
            UpdateAchievementObject(i, achievement, achievement.IsHidden);
        }
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
            UpdateAchievementStatus(achievement);
        }
    }
    private void UnlockAchievement(AchievementType achievement)
    {
        if (_queuedAchievements.Contains(achievement))
        {
            return;
        }
        achievement.UnlockAchievement();
        EventPackageFactory.BuildAndInvoke(_saveGame);
        UpdateAchievementStatus(achievement);
        AddToQueueDisplay(achievement);
        CheckAchievementTypes();
    }
    private void UpdateAchievementStatus(AchievementType achievement)
    {
        int objectIndex = _achievementObjects.FindIndex(obj => obj.AchievementId == achievement.AchievementId);
        if (objectIndex == -1)
        {
            Debug.LogWarning("No corresponding achievement object found with achievement: " + achievement.Title + "!");
            return;
        }
        bool shouldDisplayAsHidden = !achievement.IsUnlocked && achievement.IsHidden;
        UpdateAchievementObject(objectIndex, achievement, shouldDisplayAsHidden);
    }
    private void UpdateAchievementObject(int objectIndex, AchievementType achievement, bool isHidden)
    {
        AchievementObject achievementObject = _achievementObjects[objectIndex];
        if (achievement.IsUnlocked)
        {
            achievementObject.UnlockAchievement();
        }
        else if (!achievement.IsHidden)
        {
            achievementObject.EnableLock();
        }
        achievementObject.SetAchievementData(achievement.Icon, achievement.Title, achievement.Description, achievement.HasProgressionDisplay, achievement.ProgressionDisplay, isHidden);
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
            UpdateAchievementStatus(achievement);
        }
    }
    #region Achievement display
    private void AddToQueueDisplay(AchievementType achievement)
    {
        if (_queuedAchievements.Count == 0)
        {
            _queuedAchievements.Add(achievement);
            DisplayPopUpAchievement(achievement);
        }
        else
        {
            _queuedAchievements.Add(achievement);
        }
    }
    private void DisplayNextinQueue()
    {
        _queuedAchievements.RemoveAt(0);
        if (_queuedAchievements.Count != 0)
        {
            DisplayPopUpAchievement(_queuedAchievements[0]);
        }
    }
    private void DisplayPopUpAchievement(AchievementType achievement)
    {
        EventPackageFactory.BuildAndInvoke(_setAchievementPopUpInfo, achievement.Icon, achievement.Title);
        EventPackageFactory.BuildAndInvoke(_playPopUpDisplayStatus, "Displaying");
        _soundEffect = RuntimeManager.CreateInstance(achievement.SoundEffect);
        RuntimeManager.AttachInstanceToGameObject(_soundEffect, transform);
        _soundEffect.start();
        _soundEffect.release();
    }
    #endregion
    #region Co-routines
    private IEnumerator DelayUpdateUnlockedStatus(AchievementType achievement)
    {
        yield return new WaitForSeconds(0.01f);
        UpdateAchievementStatus(achievement);
    }
    private IEnumerator PopupCooldown()
    {
        yield return new WaitForSeconds(_displayPopupTime);
        EventPackageFactory.BuildAndInvoke(_playPopUpDisplayStatus, "Hiding");
        yield return new WaitForSeconds(1.5f);
        if (_queuedAchievements.Count != 0)
        {
            DisplayNextinQueue();
        }
        else
        {
            StopAllCoroutines();
        }
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
            UpdateAchievementStatus(achievement);
        }
    }
    private void SaveAchievementDataToGameData(GameData gameData)
    {
        List<AchievementType>.Enumerator enumAchievementsList = _allAchievementsListReference.AllAchievements.GetEnumerator();
        try
        {
            while (enumAchievementsList.MoveNext())
            {
                AchievementType achievement = enumAchievementsList.Current;
                achievement.SaveAchievementStatus(gameData);;
            }
        }
        finally
        {
            enumAchievementsList.Dispose();
        }
    }
    #endregion
}