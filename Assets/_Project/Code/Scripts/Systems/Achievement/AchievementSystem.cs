using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Threading.Tasks;

public class AchievementSystem : MonoBehaviour
{
    [Header("Achievement list reference")]
    [SerializeField] private AchievementSOList _achievementSOList;
    [Header("Event references")]
    [SerializeField] private SingleEvent _playPopUpDisplayStatusEvent;
    [SerializeField] private DoubleEvent _setAchievementPopUpInfoEvent;
    [SerializeField] private EmptyEvent _saveGameEvent;
    [SerializeField] private SingleEvent _updateProgressionEvent;
    [Header("Component & Settings references")]
    [SerializeField] private int _displayPopupTime = 5;
    [SerializeField] private Sprite _hiddenAchievement;
    [SerializeField] private RectTransform _achievementContainerRect;
    [SerializeField] private AchievementObject _achievementPrefabContainer;
    private List<AchievementObject> _achievementObjects = new List<AchievementObject>();
    private List<AchievementSO> _QueuedAchievements = new List<AchievementSO>();
    private readonly string _hiddenText = "??????????????";
    private EventInstance _soundEffect;
    private AchievementSO FindAchievementById(string achievementID)
    {
        for (int i = 0; i < _achievementSOList.AchievementList.Count; i++)
        {
            if (_achievementSOList.AchievementList[i] != null &&
                _achievementSOList.AchievementList[i].AchievementId == achievementID)
            {
                return _achievementSOList.AchievementList[i];
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
        foreach (AchievementSO achievement in _achievementSOList.AchievementList)
        {
            achievement.LockAchievement();
            achievement.NewCurrentValue(0);
            StartCoroutine(DeplayUpdateUnlockedStatus(achievement));
        }
    }
    public void CheckCollectableRequest(object collectableTypeObj)
    {
        CollectableTypeSO collectableType = (CollectableTypeSO)collectableTypeObj;
        Dictionary<AchievementSO, List<AchievementSO>> dependencyGraph = new Dictionary<AchievementSO, List<AchievementSO>>();
        for (int i = 0; i < _achievementSOList.AchievementList.Count; i++)
        {
            AchievementSO achievement = _achievementSOList.AchievementList[i];
            if (achievement.IsUnlocked || achievement.CompletionEnumRequirement != CompletionEnumRequirement.CollectableRequirement)
            {
                continue;
            }
            if (!achievement.IsAchievementRelated(collectableType.CollectableId))
            {
                continue;
            }
            UpdateAchievementStatus(achievement);
            if (!achievement.IsCollectableGoalReached(collectableType))
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
                if (!dependencyGraph.ContainsKey(achievement.PreviousAchievementSO))
                {
                    dependencyGraph[achievement.PreviousAchievementSO] = new List<AchievementSO>();
                }
                continue;
            }
            dependencyGraph[achievement.PreviousAchievementSO].Add(achievement);
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
    private void SetupAchievementDisplay()
    {
        if (_achievementSOList.AchievementList.Count == 0)
        {
            Debug.LogWarning("The list of achievements to unlock is empty!");
            return;
        }
        for (int i = 0; i < _achievementSOList.AchievementList.Count; i++)
        {
            AchievementSO achievement = _achievementSOList.AchievementList[i];
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
    private void UnlockAchievement(AchievementSO achievement)
    {
        if (_QueuedAchievements.Contains(achievement))
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
        if (isHidden)
        {
            achievementObject.SetAchievementData(_hiddenAchievement, _hiddenText, _hiddenText, false, string.Empty);
            return;
        }
        achievementObject.SetAchievementData(achievement.Icon, achievement.Title, achievement.Description, achievement.ShowProgression, achievement.Progression);
    }
    private void AddToQueueDisplay(AchievementSO achievement)
    {
        if (_QueuedAchievements.Count == 0)
        {
            _QueuedAchievements.Add(achievement);
            DisplayPopUpAchievement(achievement);
        }
        else
        {
            _QueuedAchievements.Add(achievement);
        }
    }
    private void CheckAchievementTypes()
    {
        for (int i =0; i < _achievementSOList.AchievementList.Count; i++)
        {
            AchievementSO achievement = _achievementSOList.AchievementList[i];
            if (achievement.CompletionEnumRequirement != CompletionEnumRequirement.AchievementRequirement)
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
    private void DisplayNextinQueue()
    {
        _QueuedAchievements.RemoveAt(0);
        if (_QueuedAchievements.Count != 0)
        {
            DisplayPopUpAchievement(_QueuedAchievements[0]);
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
        if (_QueuedAchievements.Count != 0)
        {
            DisplayNextinQueue();
        }
        else
        {
            StopAllCoroutines();
        }
    }
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
        foreach (AchievementSO achievement in _achievementSOList.AchievementList)
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
        List<AchievementSO>.Enumerator enumAchievementsList = _achievementSOList.AchievementList.GetEnumerator();
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
}