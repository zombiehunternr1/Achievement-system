using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AchievementSystem : MonoBehaviour
{
    [SerializeField] private PlayPopUpDisplayStatusEvent _playPopUpDisplayStatusEvent;
    [SerializeField] private SetAchievementPopUpInfoEvent _setAchievementPopUpInfoEvent;
    [SerializeField] private GenericEmptyEvent _saveGameEvent;
    [SerializeField] private UpdateProgressionEvent _updateProgressionEvent;
    [SerializeField] private AchievementReferenceHolderSO _overAchieverReference;
    [SerializeField] private AchievementListSO _achievementListReference;
    [SerializeField] private Sprite _hiddenAchievement;
    [SerializeField] private RectTransform _achievementContainerRect;
    [SerializeField] private AchievementObject _achievementPrefabContainer;
    [SerializeField] private int _displayPopupTime = 5;
    private List<AchievementObject> _achievementObjects;
    private List<AchievementInfoSO> _QueuedAchievements;
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
        foreach (BaseCollectableTypeSO collectable in achievement.CollectableList.CollectablesList)
        {
            if (collectable.IsCollected)
            {
                collectedCount++;
            }
        }
        return collectedCount;
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
        UpdateUnlockedStatus(achievement);
    }

    public void CheckValueRequirement(string achievementID, int? intValue, float? floatValue)
    {
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
        if (achievement.CollectableType == AchievementInfoSO.CollectableEnumType.Achievement)
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
            if (achievement.CollectableType != AchievementInfoSO.CollectableEnumType.None)
            {
                CheckCollectableType(achievement);
            }
            else
            {
                achievement.UpdateCurrentAmount(intValue, null);
                if (intValue.Value == achievement.IntGoal)
                {
                    UnlockAchievement(achievement);
                }
            }
        }
        else
        {
            achievement.UpdateCurrentAmount(null, floatValue);
            if (floatValue.Value == achievement.FloatGoal)
            {
                UnlockAchievement(achievement);
            }
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
            meetsGoal = collectedCount.Equals(achievement.CollectableList.CollectablesList.Count);
        }
        if (meetsGoal)
        {
            UnlockAchievement(achievement);
        }
        UpdateUnlockedStatus(achievement);
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
                Debug.LogWarning($"There is a missing reference at element {i} in the achievements to unlock list");
                continue;
            }
            AchievementObject achievementObject = Instantiate(_achievementPrefabContainer, _achievementContainerRect);
            achievementObject.SetAchievementId(achievement.AchievementId);
            _achievementObjects.Add(achievementObject);
            bool isHidden = achievement.IsHidden;
            if (isHidden)
            {
                achievementObject.DisableLock();
            }
            UpdateUnlockedStatus(achievement);
        }
    }
    /*
    private void UpdateUnlockedStatus()
    {
        int objectIndex = 0;

        for (int i = 0; i < _achievementListReference.AchievementList.Count; i++)
        {
            var achievement = _achievementListReference.AchievementList[i];

            if (achievement == null)
                continue;

            UpdateProgresssionStatus(i);

            bool isHidden = achievement.IsHidden;
            bool isUnlocked = achievement.IsUnlocked;
            if (isUnlocked)
            {
                HandleUnlockedAchievement(objectIndex, i);
                _achievementObjects[objectIndex].UnlockAchievement();
            }
            else
            {
                HandleLockedAchievement(objectIndex, i, isHidden);
            }
            objectIndex++;
        }
    }
    */
    private void UpdateUnlockedStatus(AchievementInfoSO achievement)
    {
        int objectIndex = _achievementObjects.FindIndex(obj => obj.AchievementId == achievement.AchievementId);
        if (objectIndex == -1)
        {
            Debug.LogWarning("No corresponding achievement object found!");
            return;
        }
        UpdateProgressionStatus(achievement);
        if (achievement.IsUnlocked)
        {
            if (achievement.IsHidden)
            {
                UpdateAchievementObject(objectIndex, achievement, false);
            }
            _achievementObjects[objectIndex].UnlockAchievement();
        }
        else
        {
            bool isHidden = achievement.IsHidden;
            UpdateAchievementObject(objectIndex, achievement, isHidden);
            if (!isHidden)
            {
                _achievementObjects[objectIndex].EnableLock();
            }
        }
        UpdateRelatedAchievements(achievement);
    }
    private void UpdateRelatedAchievements(AchievementInfoSO achievement)
    {
        List<int> relatedIndexes = new List<int>();
        foreach (AchievementInfoSO relatedAchievement in _achievementListReference.AchievementList)
        {
            if(relatedAchievement != null && relatedAchievement.AchievementId.Equals(achievement.AchievementId))
            {
                continue;
            }
            int relatedIndex = -1;
            relatedIndex = _achievementObjects.FindIndex(obj => obj.AchievementId == relatedAchievement.AchievementId);
            if(relatedIndex == -1)
            {
                continue;
            }
            relatedIndexes.Add(relatedIndex);
        }
        foreach (int relatedIndex in relatedIndexes) 
        {
            AchievementInfoSO relatedAchievement = _achievementListReference.AchievementList[relatedIndex];
            UpdateProgressionStatus(relatedAchievement);
            if (relatedAchievement.IsUnlocked && relatedAchievement.IsHidden)
            {
                UpdateAchievementObject(relatedIndex, relatedAchievement, false);
            }
            else
            {
                UpdateAchievementObject(relatedIndex, relatedAchievement, relatedAchievement.IsHidden);
            }
        }
    }
    private void UpdateProgressionStatus(AchievementInfoSO achievement)
    {
        if (achievement.CollectableType == AchievementInfoSO.CollectableEnumType.None)
        {
            return;
        }
        else if (achievement.CollectableType == AchievementInfoSO.CollectableEnumType.Collectable)
        {
            _intAmount = 0;
            if (achievement.CollectableRequirementType == AchievementInfoSO.CollectableRequirementEnumType.Single)
            {
                if (achievement.Collectable.IsCollected)
                {
                    _intAmount++;
                }
            }
            else
            {
                foreach (CollectableTypeSO collectable in achievement.CollectableList.CollectablesList)
                {
                    if (collectable.IsCollected)
                    {
                        _intAmount++;
                    }
                }
            }
        }
        else if (achievement.CollectableType == AchievementInfoSO.CollectableEnumType.Achievement)
        {
            _intAmount = 0;
            foreach (AchievementInfoSO subAchievement in achievement.Achievements.AchievementList)
            {
                if(subAchievement.CollectableType != AchievementInfoSO.CollectableEnumType.Achievement)
                {
                    if (subAchievement.IsUnlocked)
                    {
                        _intAmount++;
                    }
                }
            }
        }
    }
    private void UpdateAchievementObject(int objectIndex, AchievementInfoSO achievement, bool isHidden)
    {
        if (isHidden)
        {
            _achievementObjects[objectIndex].SetIcon(_hiddenAchievement);
            _achievementObjects[objectIndex].SetTitle(_hiddenText);
            _achievementObjects[objectIndex].SetDescription(_hiddenText);
            _achievementObjects[objectIndex].ProgressDisplay(false, achievement.CurrentIntAmount, achievement.IntGoal,
            achievement.CurrentFloatAmount, achievement.FloatGoal);
        }
        else
        {
            _achievementObjects[objectIndex].SetIcon(achievement.Icon);
            _achievementObjects[objectIndex].SetTitle(achievement.Title);
            _achievementObjects[objectIndex].SetDescription(achievement.Description);
            if (achievement.ShowProgression)
            {
                if (achievement.ManualGoalAmount)
                {
                    _achievementObjects[objectIndex].ProgressDisplay(true, _intAmount, achievement.IntGoal,
                    achievement.CurrentFloatAmount, achievement.FloatGoal);
                }
                else
                {
                    if (achievement.CollectableType == AchievementInfoSO.CollectableEnumType.Collectable)
                    {
                        _achievementObjects[objectIndex].ProgressDisplay(true, _intAmount, achievement.CollectableList.CollectablesList.Count,
                        achievement.CurrentFloatAmount, achievement.FloatGoal);
                    }
                    else if (achievement.CollectableType == AchievementInfoSO.CollectableEnumType.Achievement)
                    {
                        _achievementObjects[objectIndex].ProgressDisplay(true, _intAmount, achievement.AchievementCount,
                        achievement.CurrentFloatAmount, achievement.FloatGoal);
                    }
                }
            }
            else
            {
                _achievementObjects[objectIndex].ProgressDisplay(false, _intAmount, achievement.IntGoal,
                achievement.CurrentFloatAmount, achievement.FloatGoal);
            }
        }
    }
    private void UnlockAchievement(AchievementInfoSO achievement)
    {
        achievement.AchievementUnlocked = true;
        _saveGameEvent.Invoke();
        UpdateUnlockedStatus(achievement);
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
    public void UpdateData(GameData data, bool isLoading)
    {
        if (isLoading)
        {
            LoadAchievementDataFromGameData(data);
        }
        else
        {
            SaveAchievementDataToGameData(data);
        }
        _updateProgressionEvent.Invoke(data);
    }
    private void LoadAchievementDataFromGameData(GameData data)
    {
        foreach (AchievementInfoSO achievement in _achievementListReference.AchievementList)
        {
            data.TotalAchievementsData.TryGetValue(achievement.AchievementId, out bool isUnlocked);
            achievement.AchievementUnlocked = isUnlocked;
            UpdateUnlockedStatus(achievement);
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