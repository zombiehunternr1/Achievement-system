using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Threading.Tasks;

public class AchievementSystem : MonoBehaviour
{
    [Header("Achievement list reference")]
    [SerializeField] private AchievementSOList _allAchievementsListReference;
    [Header("Event references")]
    [SerializeField] private SingleEvent _playPopUpDisplayStatusEvent;
    [SerializeField] private DoubleEvent _setAchievementPopUpInfoEvent;
    [SerializeField] private EmptyEvent _saveGameEvent;
    [SerializeField] private SingleEvent _updateProgressionEvent;
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
            for (int i = _queuedAchievements.Count - 1; i > 0; i--)
            {
                _queuedAchievements.RemoveAt(i);
            }
        }
        foreach (AchievementSO achievement in _allAchievementsListReference.AchievementList)
        {
            achievement.LockAchievement();
            achievement.NewCurrentValue(0);
            StartCoroutine(DeplayUpdateUnlockedStatus(achievement));
        }
        _saveGameEvent.Invoke();
    }
    public void CheckCollectableRequest(object collectableV2Obj)
    {
        CollectableSO collectableV2 = (CollectableSO)collectableV2Obj;
        Dictionary<AchievementSO, List<AchievementSO>> dependencyGraph = new Dictionary<AchievementSO, List<AchievementSO>>();
        for (int i = 0; i < _allAchievementsListReference.AchievementList.Count; i++)
        {
            AchievementSO achievement = _allAchievementsListReference.AchievementList[i];
            if (achievement.IsUnlocked || achievement.CompletionEnumRequirement != CompletionEnumRequirement.CollectableRequirement)
            {
                continue;
            }
            if (!achievement.IsAchievementRelated(collectableV2))
            {
                continue;
            }
            UpdateAchievementStatus(achievement);
            if (!achievement.IsCollectableGoalReached(collectableV2))
            {
                continue;
            }
            if (!dependencyGraph.ContainsKey(achievement))
            {
                dependencyGraph[achievement] = new List<AchievementSO>();
                continue;
            }
            if (achievement.RequiresPreviousAchievement && achievement.PreviousAchievementUnlocked)
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
    public void UpdateRecievedAchievement(object achievementIDObj, object valueObj)
    {
        string achievementID = (string)achievementIDObj;
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
        achievement.NewCurrentValue(valueObj);
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
        _saveGameEvent.Invoke();
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
        achievementObject.SetAchievementData(achievement.Icon, achievement.Title, achievement.Description, achievement.ShowProgression, achievement.Progression, isHidden);
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
        _setAchievementPopUpInfoEvent.Invoke(achievement.Icon, achievement.Title);
        _playPopUpDisplayStatusEvent.Invoke("Displaying");
        _soundEffect = RuntimeManager.CreateInstance(achievement.SoundEffect);
        RuntimeManager.AttachInstanceToGameObject(_soundEffect, transform);
        _soundEffect.start();
        _soundEffect.release();
    }
    #endregion
    #region Co-routines
    private IEnumerator DeplayUpdateUnlockedStatus(AchievementSO achievement)
    {
        yield return new WaitForSeconds(0.01f);
        UpdateAchievementStatus(achievement);
    }
    private IEnumerator PopupCooldown()
    {
        yield return new WaitForSeconds(_displayPopupTime);
        _playPopUpDisplayStatusEvent.Invoke("Hiding");
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
    public async void UpdateData(object gameDataObj, object isLoadingObj)
    {
        GameData gameData = (GameData)gameDataObj;
        bool isLoading = (bool)isLoadingObj;
        if (isLoading)
        {
            await Task.Delay(1);
            LoadAchievementDataFromGameData(gameData);
        }
        else
        {
            SaveAchievementDataToGameData(gameData);
        }
        _updateProgressionEvent.Invoke(gameData);
    }
    private void LoadAchievementDataFromGameData(GameData data)
    {
        foreach (AchievementSO achievement in _allAchievementsListReference.AchievementList)
        {
            data.TotalAchievementsData.TryGetValue(achievement.AchievementId, out bool isUnlocked);
            if (achievement.CompletionEnumRequirement == CompletionEnumRequirement.ValueRequirement)
            {
                data.CurrentValueData.TryGetValue(achievement.AchievementId, out float currentValue);
                achievement.NewCurrentValue(currentValue);
            }
            if (isUnlocked)
            {
                achievement.UnlockAchievement();
            }
            else
            {
                achievement.LockAchievement();
            }
            UpdateAchievementStatus(achievement);
        }
    }
    private void SaveAchievementDataToGameData(GameData data)
    {
        List<AchievementSO>.Enumerator enumAchievementsList = _allAchievementsListReference.AchievementList.GetEnumerator();
        try
        {
            while (enumAchievementsList.MoveNext())
            {
                string id = enumAchievementsList.Current.AchievementId;
                bool boolValue = enumAchievementsList.Current.IsUnlocked;
                data.SetTotalAchievementsData(id, boolValue);
                if (enumAchievementsList.Current.CompletionEnumRequirement == CompletionEnumRequirement.ValueRequirement)
                {
                    data.SetCurrentValueData(id, enumAchievementsList.Current.GetCurrentAmount);
                }
            }
        }
        finally
        {
            enumAchievementsList.Dispose();
        }
    }
    #endregion
}