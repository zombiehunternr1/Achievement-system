using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AchievementSystem : MonoBehaviour
{
    [SerializeField] private SingleEvent _playPopUpDisplayStatusEvent;
    [SerializeField] private DoubleEvent _setAchievementPopUpInfoEvent;
    [SerializeField] private EmptyEvent _saveGameEvent;
    [SerializeField] private SingleEvent _updateProgressionEvent;
    [SerializeField] private AchievementInfoSO _overAchieverReference;
    [SerializeField] private AchievementListSO _achievementListReference;
    [SerializeField] private Sprite _hiddenAchievement;
    [SerializeField] private RectTransform _achievementContainerRect;
    [SerializeField] private AchievementObject _achievementPrefabContainer;
    [SerializeField] private int _displayPopupTime = 5;
    private List<AchievementObject> _achievementObjects;
    private List<AchievementInfoSO> _QueuedAchievements;
    private BaseCollectableTypeSO _lastCollectedType;
    private int _intAmount = 0;
    private readonly string _hiddenText = "??????????????";
    private EventInstance _soundEffect;
    private int CountAllUnlockedAchievements()
    {
        int unlockedCount = 0;
        foreach (AchievementInfoSO subAchievement in _achievementListReference.AchievementList)
        {
            if (subAchievement.CollectableType != AchievementInfoSO.CollectableEnumType.Achievement && subAchievement.IsUnlocked)
            {
                unlockedCount++;
            }
        }
        return unlockedCount;
    }
    private int CountCollectedItems(AchievementInfoSO achievement)
    {
        int collectedCount = 0;
        foreach (BaseCollectableTypeSO collectable in achievement.CollectableList)
        {
            if (collectable.IsCollected)
            {
                collectedCount++;
            }
        }
        return collectedCount;
    }
    private int AddCollectedAmount(AchievementInfoSO achievement)
    {
        int amount = 0;
        if (achievement.CollectableRequirementType == AchievementInfoSO.CollectableRequirementEnumType.Single)
        {
            if (achievement.Collectable.IsCollected)
            {
                amount++;
            }
        }
        else
        {
            foreach (CollectableTypeSO collectable in achievement.CollectableList)
            {
                if (collectable.IsCollected)
                {
                    amount++;
                }
            }
        }
        return amount;
    }
    private int CountUnlockedNonAchievementItems()
    {
        int amount = 0;
        foreach (AchievementInfoSO achievement in _achievementListReference.AchievementList)
        {
            if (achievement.CollectableType != AchievementInfoSO.CollectableEnumType.Achievement && achievement.IsUnlocked)
            {
                amount++;
            }
        }
        return amount;
    }
    private int FindAchievementObjectIndex(string achievementId)
    {
        for (int j = 0; j < _achievementObjects.Count; j++)
        {
            if (_achievementObjects[j].AchievementId == achievementId)
            {
                return j;
            }
        }
        return -1;
    }
    private bool IsRelevantAchievement(AchievementInfoSO relatedAchievement, AchievementInfoSO achievement)
    {
        if (relatedAchievement.CollectableType == AchievementInfoSO.CollectableEnumType.Achievement)
        {
            return true;
        }
        if (relatedAchievement.CollectableType == AchievementInfoSO.CollectableEnumType.None ||
            achievement.CollectableType == AchievementInfoSO.CollectableEnumType.None ||
            relatedAchievement.CollectableRequirementType == AchievementInfoSO.CollectableRequirementEnumType.Single ||
            achievement.CollectableRequirementType == AchievementInfoSO.CollectableRequirementEnumType.Single)
        {
            return false;
        }
        if (relatedAchievement.ManualGoalAmount || _lastCollectedType == null)
        {
            return false;
        }
        foreach (BaseCollectableTypeSO collectable in achievement.CollectableList)
        {
            foreach (BaseCollectableTypeSO relatedCollectable in relatedAchievement.CollectableList)
            {
                if (relatedCollectable.CollectableId == _lastCollectedType.CollectableId)
                {
                    _lastCollectedType = null;
                    return true;
                }
            }
        }
        return false;
    }
    private AchievementInfoSO FindAchievementById(string achievementID)
    {
        for (int i = 0; i < _achievementListReference.AchievementList.Count; i++)
        {
            if (_achievementListReference.AchievementList[i] != null &&
                _achievementListReference.AchievementList[i].AchievementId == achievementID)
            {
                return _achievementListReference.AchievementList[i];
            }
        }
        return null;
    }
    private void Awake()
    {
        _achievementObjects = new List<AchievementObject>();
        _QueuedAchievements = new List<AchievementInfoSO>();
        SetupAchievementDisplay();
    }
    public void StartPopupCooldown()
    {
        StartCoroutine(PopupCooldown());
    }
    public void ResetAllAchievements()
    {
        foreach(AchievementInfoSO achievement in _achievementListReference.AchievementList)
        {
            achievement.AchievementUnlocked = false;
            StartCoroutine(DeplayUpdateUnlockedStatus(achievement));
        }
    }
    private IEnumerator DeplayUpdateUnlockedStatus(AchievementInfoSO achievement)
    {
        yield return new WaitForSeconds(0.01f);
        UpdateAchievementStatus(achievement);
    }
    public void CheckCollectableRequest(object collectableTypeListObj, object collectableTypeObj, object collectedAmountObj)
    {
        CollectableTypeListSO collectableTypeList = (CollectableTypeListSO)collectableTypeListObj;
        CollectableTypeSO collectableType = (CollectableTypeSO)collectableTypeObj;
        int collectedAmount = (int)collectedAmountObj;
        foreach (AchievementInfoSO achievement in _achievementListReference.AchievementList)
        {
            bool hasCollectableTypeList = achievement.CollectableList != null;
            bool hasCollectables = hasCollectableTypeList && achievement.CollectableList.Count > 0;
            if (hasCollectables && !collectableTypeList.CollectablesList.Contains(collectableType))
            {
                continue;
            }
            if (!hasCollectables && achievement.Collectable != collectableType)
            {
                continue;
            }
            _lastCollectedType = collectableType;
            CheckValueRequirement(achievement.AchievementId, collectedAmount, null);
        }
    }
    public void CheckValueRequirement(object achievementIDObj, object intValueObj, object floatValueObj)
    {
        string achievementID = (string)achievementIDObj;
        int? intValue = intValueObj as int?;
        float? floatValue = floatValueObj as float?;
        AchievementInfoSO achievement = FindAchievementById(achievementID);
        if (achievement == null)
        {
            Debug.LogWarning("Couldn't find the achievement in the list with ID: " + achievementID);
            return;
        }
        if (achievement.IsUnlocked)
        {
            return;
        }
        if (achievement.CompletionType == AchievementInfoSO.CompletionEnumType.NoRequirements)
        {
            UnlockAchievement(achievement);
            return;
        }
        if (achievement.CollectableType == AchievementInfoSO.CollectableEnumType.Achievement || 
            achievement.CollectableType == AchievementInfoSO.CollectableEnumType.Collectable)
        {
            CheckCollectableType(achievement);
            return;
        }
        HandleValueUpdate(achievement, intValue, floatValue);
    }
    private void HandleValueUpdate(AchievementInfoSO achievement, int? intValue, float? floatValue)
    {
        if (intValue.HasValue)
        {
            achievement.UpdateCurrentAmount(intValue, null);
            if (intValue.Value == achievement.IntGoal)
            {
                UnlockAchievement(achievement);
            }
            return;
        }
        achievement.UpdateCurrentAmount(null, floatValue);
        if (floatValue.Value == achievement.FloatGoal)
        {
            UnlockAchievement(achievement);
        }
    }
    private void CheckCollectableType(AchievementInfoSO achievement)
    {
        if (achievement.CollectableType == AchievementInfoSO.CollectableEnumType.Achievement)
        {
            int unlockedCount = CountAllUnlockedAchievements();
            if (unlockedCount >= achievement.AchievementCount)
            {
                UnlockAchievement(achievement);
            }
            return;
        }
        if (achievement.CollectableRequirementType == AchievementInfoSO.CollectableRequirementEnumType.Single)
        {
            if (!achievement.Collectable.IsCollected)
            {
                return;
            }
            if (achievement.RequiresPreviousAchievement && !achievement.PreviousAchievement.IsUnlocked)
            {
                return;
            }
            UnlockAchievement(achievement);
            return;
        }
        int collectedCount = CountCollectedItems(achievement);
        bool meetsGoal;
        if (achievement.ManualGoalAmount)
        {
            meetsGoal = collectedCount.Equals(achievement.IntGoal);
        }
        else
        {
            meetsGoal = collectedCount.Equals(achievement.CollectableList.Count);
        }
        if (meetsGoal)
        {
            UnlockAchievement(achievement);
            return;
        }
        UpdateAchievementStatus(achievement);
    }
    private void SetupAchievementDisplay()
    {
        if (_achievementListReference.AchievementList.Count == 0)
        {
            Debug.LogWarning("The list of achievements to unlock is empty!");
            return;
        }
        for (int i = 0; i < _achievementListReference.AchievementList.Count; i++)
        {
            AchievementInfoSO achievement = _achievementListReference.AchievementList[i];
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
    private void UpdateAchievementStatus(AchievementInfoSO achievement)
    {
        int objectIndex = _achievementObjects.FindIndex(obj => obj.AchievementId == achievement.AchievementId);
        if (objectIndex == -1)
        {
            Debug.LogWarning("No corresponding achievement object found with achievement: " + achievement.Title + "!");
            return;
        }
        UpdateProgressionStatus(achievement);
        bool shouldDisplayAsHidden = !achievement.IsUnlocked && achievement.IsHidden;
        UpdateAchievementObject(objectIndex, achievement, shouldDisplayAsHidden);
        if (achievement.IsUnlocked)
        {
            _achievementObjects[objectIndex].UnlockAchievement();
        }
        else if (!achievement.IsHidden)
        {
            _achievementObjects[objectIndex].EnableLock();
        }
        UpdateRelatedAchievements(achievement);
    }
    private void UpdateRelatedAchievements(AchievementInfoSO achievement)
    {
        Dictionary<int, AchievementInfoSO> relatedAchievementDictionary = new Dictionary<int, AchievementInfoSO>();
        foreach (AchievementInfoSO relatedAchievement in _achievementListReference.AchievementList)
        {
            if (relatedAchievement == null || !IsRelevantAchievement(relatedAchievement, achievement))
            {
                continue;
            }
            int index = FindAchievementObjectIndex(relatedAchievement.AchievementId);
            if (index == -1)
            {
                Debug.LogWarning("Couldn't find the achievement object for ID: " + relatedAchievement.AchievementId + "!");
                continue;
            }
            relatedAchievementDictionary.Add(index, relatedAchievement);
        }
        foreach (KeyValuePair<int, AchievementInfoSO> keyValuePair in relatedAchievementDictionary)
        {
            UpdateAchievementProgress(keyValuePair.Key, keyValuePair.Value);
        }
    }
    private void UpdateAchievementProgress(int index, AchievementInfoSO relatedAchievement)
    {
        UpdateProgressionStatus(relatedAchievement);
        UpdateAchievementObject(index, relatedAchievement, relatedAchievement.IsHidden);
        if (relatedAchievement.IsUnlocked)
        {
            _achievementObjects[index].UnlockAchievement();
        }
        else if (!relatedAchievement.IsHidden)
        {
            _achievementObjects[index].EnableLock();
        }
    }
    private void UpdateProgressionStatus(AchievementInfoSO achievement)
    {
        if (achievement.CollectableType == AchievementInfoSO.CollectableEnumType.None)
        {
            return;
        }
        if (achievement.CollectableType == AchievementInfoSO.CollectableEnumType.Collectable)
        {
            _intAmount = AddCollectedAmount(achievement);
            return;
        }
        _intAmount = CountUnlockedNonAchievementItems();
    }
    private void UpdateAchievementObject(int objectIndex, AchievementInfoSO achievement, bool isHidden)
    {
        AchievementObject achievementObject = _achievementObjects[objectIndex];
        if (isHidden)
        {
            achievementObject.SetIcon(_hiddenAchievement);
            achievementObject.SetTitle(_hiddenText);
            achievementObject.SetDescription(_hiddenText);
            achievementObject.ProgressDisplay(false, 0,0,0,0);
            return;
        }
        achievementObject.SetIcon(achievement.Icon);
        achievementObject.SetTitle(achievement.Title);
        achievementObject.SetDescription(achievement.Description);
        if (!achievement.ShowProgression)
        {
            achievementObject.ProgressDisplay(false, 0, 0, 0, 0);
            return;
        }
        if (achievement.ManualGoalAmount)
        {
            achievementObject.ProgressDisplay(true, _intAmount, achievement.IntGoal,
            achievement.CurrentFloatAmount, achievement.FloatGoal);
            return;
        }
        if (achievement.CollectableType == AchievementInfoSO.CollectableEnumType.Collectable)
        {
            achievementObject.ProgressDisplay(true, _intAmount, achievement.CollectableList.Count,
            achievement.CurrentFloatAmount, achievement.FloatGoal);
            return;
        }
        achievementObject.ProgressDisplay(true, _intAmount, achievement.AchievementCount,
        achievement.CurrentFloatAmount, achievement.FloatGoal);
    }
    private void UnlockAchievement(AchievementInfoSO achievement)
    {
        achievement.AchievementUnlocked = true;
        _saveGameEvent.Invoke();
        UpdateAchievementStatus(achievement);
        AddToQueueDisplay(achievement);
    }
    private void AddToQueueDisplay(AchievementInfoSO achievement)
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
        CheckValueRequirement(_overAchieverReference.AchievementId, null, null);
    }
    private void DisplayNextinQueue()
    {
        _QueuedAchievements.RemoveAt(0);
        if (_QueuedAchievements.Count != 0)
        {
            DisplayPopUpAchievement(_QueuedAchievements[0]);
        }
    }
    private void DisplayPopUpAchievement(AchievementInfoSO achievement)
    {
        _setAchievementPopUpInfoEvent.Invoke(achievement.Icon, achievement.Title);
        _playPopUpDisplayStatusEvent.Invoke("Displaying");
        _soundEffect = RuntimeManager.CreateInstance(achievement.SoundEffect);
        RuntimeManager.AttachInstanceToGameObject(_soundEffect, transform);
        _soundEffect.start();
        _soundEffect.release();
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
    public void UpdateData(object gameDataObj, object isLoadingObj)
    {

        GameData gameData = (GameData)gameDataObj;
        bool isLoading = (bool)isLoadingObj;
        if (isLoading)
        {
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
        foreach (AchievementInfoSO achievement in _achievementListReference.AchievementList)
        {
            data.TotalAchievementsData.TryGetValue(achievement.AchievementId, out bool isUnlocked);
            achievement.AchievementUnlocked = isUnlocked;
            UpdateAchievementStatus(achievement);
        }
    }
    private void SaveAchievementDataToGameData(GameData data)
    {
        List<AchievementInfoSO>.Enumerator enumAchievementsList = _achievementListReference.AchievementList.GetEnumerator();

        try
        {
            while (enumAchievementsList.MoveNext())
            {
                string id = enumAchievementsList.Current.AchievementId;
                bool value = enumAchievementsList.Current.IsUnlocked;
                data.SetTotalAchievementsData(id, value);
            }
        }
        finally
        {
            enumAchievementsList.Dispose();
        }
    }
}