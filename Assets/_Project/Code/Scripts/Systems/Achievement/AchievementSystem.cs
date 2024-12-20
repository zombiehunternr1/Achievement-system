using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AchievementSystem : MonoBehaviour
{
    [Header("Achievement list reference")]
    [SerializeField] private AchievementSOList _allAchievementsListReference;
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
    private List<AchievementSO> _queuedAchievements = new List<AchievementSO>();
    private EventInstance _soundEffect;
    private AchievementSO FindAchievementById(string achievementID)
    {
        for (int i = 0; i < _allAchievementsListReference.AchievementList.Count; i++)
        {
            if (_allAchievementsListReference.AchievementList[i] != null &&
                _allAchievementsListReference.AchievementList[i].AchievementId == achievementID)
            {
                return _allAchievementsListReference.AchievementList[i];
            }
        }
        return null;
    }
    private List<AchievementSO> TopologicalSort(Dictionary<AchievementSO, List<AchievementSO>> dependencyGraph)
    {
        List<AchievementSO> sortedList = new List<AchievementSO>();
        HashSet<AchievementSO> visited = new HashSet<AchievementSO>();
        void Visit(AchievementSO achievement)
        {
            if (!visited.Contains(achievement))
            {
                visited.Add(achievement);
                foreach (AchievementSO neighbor in dependencyGraph[achievement])
                {
                    Visit(neighbor);
                }
                sortedList.Add(achievement);
            }
        }
        foreach (AchievementSO achievement in dependencyGraph.Keys)
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
        foreach (AchievementSO achievement in _allAchievementsListReference.AchievementList)
        {
            achievement.LockAchievement();
            achievement.SetCurrentValue(0);
            StartCoroutine(DelayUpdateUnlockedStatus(achievement));
        }
        EventPackageFactory.BuildAndInvoke(_saveGame);
    }
    public void CheckCollectableRequest(EventData eventData)
    {
        CollectableSO collectable = EventPackageExtractor.ExtractEventData<CollectableSO>(eventData);
        Dictionary<AchievementSO, List<AchievementSO>> dependencyGraph = new Dictionary<AchievementSO, List<AchievementSO>>();
        for (int i = 0; i < _allAchievementsListReference.AchievementList.Count; i++)
        {
            AchievementSO achievement = _allAchievementsListReference.AchievementList[i];
            if (achievement.IsUnlocked || achievement.CompletionEnumRequirement != CompletionEnumRequirement.CollectableRequirement)
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
                dependencyGraph[achievement] = new List<AchievementSO>();
                continue;
            }
            if (achievement.RequiresPreviousAchievement && achievement.IsPreviousAchievementUnlocked)
            {
                if (!dependencyGraph.ContainsKey(achievement.PreviousAchievement))
                {
                    dependencyGraph[achievement.PreviousAchievement] = new List<AchievementSO>();
                }
                continue;
            }
            dependencyGraph[achievement.PreviousAchievement].Add(achievement);
        }
        List<AchievementSO> sortedAchievements = TopologicalSort(dependencyGraph);
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
        AchievementSO achievement = FindAchievementById(achievementID);
        if (achievement == null)
        {
            Debug.LogWarning("Couldn't find the achievement in the list with ID: " + achievementID);
            return;
        }
        CheckAchievementRequirementStatus(achievement, valueObj);
    }
    private void SetupAchievementDisplay()
    {
        if (_allAchievementsListReference.AchievementList.Count == 0)
        {
            Debug.LogWarning("The list of achievements to unlock is empty!");
            return;
        }
        for (int i = 0; i < _allAchievementsListReference.AchievementList.Count; i++)
        {
            AchievementSO achievement = _allAchievementsListReference.AchievementList[i];
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
    private void CheckAchievementRequirementStatus(AchievementSO achievement, object valueObj)
    {
        if (achievement.IsUnlocked)
        {
            return;
        }
        if (achievement.CompletionEnumRequirement == CompletionEnumRequirement.NoRequirement ||
            achievement.CompletionEnumRequirement == CompletionEnumRequirement.AchievementRequirement && achievement.IsAchievementGoalReached)
        {
            UnlockAchievement(achievement);
            return;
        }
        if (achievement.CompletionEnumRequirement == CompletionEnumRequirement.ValueRequirement)
        {
            HandleValueUpdate(achievement, valueObj);
            return;
        }
    }
    private void HandleValueUpdate(AchievementSO achievement, object valueObj)
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
    private void UnlockAchievement(AchievementSO achievement)
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
    private void UpdateAchievementStatus(AchievementSO achievement)
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
    private void UpdateAchievementObject(int objectIndex, AchievementSO achievement, bool isHidden)
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
        achievementObject.SetAchievementData(achievement.Icon, achievement.Title, achievement.Description, achievement.HasProgressionDisplay, achievement.GetProgressionDisplay, isHidden);
    }
    private void CheckAchievementTypes()
    {
        for (int i =0; i < _allAchievementsListReference.AchievementList.Count; i++)
        {
            AchievementSO achievement = _allAchievementsListReference.AchievementList[i];
            if (achievement.CompletionEnumRequirement != CompletionEnumRequirement.AchievementRequirement || achievement.IsUnlocked)
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
    private void AddToQueueDisplay(AchievementSO achievement)
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
    private void DisplayPopUpAchievement(AchievementSO achievement)
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
    private IEnumerator DelayUpdateUnlockedStatus(AchievementSO achievement)
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
        foreach (AchievementSO achievement in _allAchievementsListReference.AchievementList)
        {
            gameData.AchievementsData.TryGetValue(achievement.AchievementId, out AchievementDTO achievementDTO);
            achievement.LoadAchievementStatus(achievementDTO);
            UpdateAchievementStatus(achievement);
        }
    }
    private void SaveAchievementDataToGameData(GameData gameData)
    {
        List<AchievementSO>.Enumerator enumAchievementsList = _allAchievementsListReference.AchievementList.GetEnumerator();
        try
        {
            while (enumAchievementsList.MoveNext())
            {
                AchievementSO achievement = enumAchievementsList.Current;
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