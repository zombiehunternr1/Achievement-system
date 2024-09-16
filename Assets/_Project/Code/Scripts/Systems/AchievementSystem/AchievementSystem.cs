using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Linq;

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
        foreach (var subAchievement in _achievementListReference.AchievementList)
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
        foreach (var collectable in achievement.CollectableList.CollectablesList)
        {
            if (collectable.IsCollected)
            {
                collectedCount++;
            }
        }
        return collectedCount;
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
        }
        Invoke("UpdateUnlockedStatus", 0.01f);
    }
    public void CheckValueRequirement(string achievementID, int? intValue, float? floatValue)
    {
        AchievementInfoSO achievement = _achievementListReference.AchievementList
            .FirstOrDefault(achievement => achievement != null && achievement.AchievementId == achievementID);

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
        if (intValue != null)
        {
            if (achievement.CollectableType != AchievementInfoSO.CollectableEnumType.None)
            {
                CheckCollectableType(achievement);
                return;
            }
            else
            {
                achievement.UpdateCurrentAmount(intValue, null);
                if (intValue == achievement.IntGoal)
                {
                    UnlockAchievement(achievement);
                }
            }
            return;
        }
        else if (floatValue != null)
        {
            achievement.UpdateCurrentAmount(null, floatValue);
            if (floatValue == achievement.FloatGoal)
            {
                UnlockAchievement(achievement);
            }
            return;
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
            AddToQueueDisplay(achievement);
            return;
        }
        int collectedCount = CountCollectedItems(achievement);
        bool meetsGoal;
        if (achievement.ManualGoalAmount)
        {
            meetsGoal = collectedCount == achievement.IntGoal;
        }
        else
        {
            meetsGoal = collectedCount == achievement.CollectableList.CollectablesList.Count;
        }
        if (meetsGoal)
        {
            UnlockAchievement(achievement);
        }
    }
    /*
    private void CheckCollectableType(AchievementInfoSO achievement)
    {
        if (achievement.CollectableType == AchievementInfoSO.CollectableEnumType.Achievement)
        {
            int unlockedCount = _achievementListReference.AchievementList
                .Where(subAchievement => subAchievement.CollectableType != AchievementInfoSO.CollectableEnumType.Achievement
                && subAchievement.IsUnlocked).Count();
            if (unlockedCount == achievement.AchievementCount)
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
            if (!achievement.RequiresPreviousAchievement)
            {
                UnlockAchievement(achievement);
                return;
            }
            if (achievement.PreviousAchievement.IsUnlocked)
            {
                AddToQueueDisplay(achievement);
                return;
            }
        }
        else
        {
            int unlockedCount = achievement.CollectableList.CollectablesList
            .Where(collectable => collectable.IsCollected)
            .Count();
            if ((achievement.ManualGoalAmount && unlockedCount == achievement.IntGoal) ||
                (!achievement.ManualGoalAmount && unlockedCount == achievement.CollectableList.CollectablesList.Count))
            {
                UnlockAchievement(achievement);
            }
        }
    }
    */
    private void SetupAchievementDisplay()
    {
        if (_achievementListReference.AchievementList.Count == 0)
        {
            Debug.LogWarning("The list of achievements to unlock is empty!");
            return;
        }
        for (int i = 0; i < _achievementListReference.AchievementList.Count; i++)
        {
            var achievement = _achievementListReference.AchievementList[i];
            if (achievement == null)
            {
                Debug.LogWarning($"There is a missing reference at element {i} in the achievements to unlock list");
                continue;
            }
            var achievementObject = Instantiate(_achievementPrefabContainer, _achievementContainerRect);
            _achievementObjects.Add(achievementObject);
            bool isHidden = achievement.IsHidden;
            UpdateAchievementObject(_achievementObjects.Count - 1, i, isHidden);

            if (isHidden)
            {
                achievementObject.DisableLock();
            }
        }
        UpdateUnlockedStatus();
    }
    /*
    private void SetupAchievementDisplay()
    {
        if (_achievementListReference.AchievementList.Count == 0)
        {
            Debug.LogWarning("The list of achievements to unlock is empty!");
            return;
        }
        for (int i = 0; i < _achievementListReference.AchievementList.Count; i++)
        {
            if (_achievementListReference.AchievementList[i] != null)
            {
                AchievementObject achievementObject = Instantiate(_achievementPrefabContainer, _achievementContainerRect);
                _achievementObjects.Add(achievementObject);
                if (_achievementListReference.AchievementList[i].IsHidden)
                {
                    UpdateAchievementObject(_achievementObjects.LastIndexOf(achievementObject), i, true);
                    achievementObject.DisableLock();
                }
                else
                {
                    UpdateAchievementObject(_achievementObjects.LastIndexOf(achievementObject), i, false);
                }
            }
            else
            {
                Debug.LogWarning("There is a missing reference at element " + i + " in the achievements to unlock list");
            }
        }
        UpdateUnlockedStatus();
    }
    */
    private void UpdateUnlockedStatus()
    {
        int objectIndex = 0;
        for (int i = 0; i < _achievementListReference.AchievementList.Count; i++)
        {
            if (_achievementListReference.AchievementList[i] != null)
            {
                UpdateProgresssionStatus(i);
                if (!_achievementListReference.AchievementList[i].IsUnlocked)
                {
                    if (_achievementListReference.AchievementList[i].IsHidden)
                    {
                        UpdateAchievementObject(objectIndex, i, true);
                    }
                    else
                    {
                        UpdateAchievementObject(objectIndex, i, false);
                        _achievementObjects[objectIndex].EnableLock();
                    }
                }
                else
                {
                    UpdateAchievementObject(objectIndex, i, false);
                    _achievementObjects[objectIndex].UnlockAchievement();
                }
                objectIndex++;
            }
        }
    }
    private void UpdateProgresssionStatus(int achievementIndex)
    {
        if (_achievementListReference.AchievementList[achievementIndex].CollectableType == AchievementInfoSO.CollectableEnumType.None)
        {
            return;
        }
        else if (_achievementListReference.AchievementList[achievementIndex].CollectableType == AchievementInfoSO.CollectableEnumType.Collectable)
        {
            _intAmount = 0;
            if (_achievementListReference.AchievementList[achievementIndex].CollectableRequirementType == AchievementInfoSO.CollectableRequirementEnumType.Single)
            {
                if (_achievementListReference.AchievementList[achievementIndex].Collectable.IsCollected)
                {
                    _intAmount++;
                }
            }
            else
            {
                foreach (CollectableTypeSO collectable in _achievementListReference.AchievementList[achievementIndex].CollectableList.CollectablesList)
                {
                    if (collectable.IsCollected)
                    {
                        _intAmount++;
                    }
                }
            }
        }
        else if (_achievementListReference.AchievementList[achievementIndex].CollectableType == AchievementInfoSO.CollectableEnumType.Achievement)
        {
            _intAmount = 0;
            foreach (AchievementInfoSO achievement in _achievementListReference.AchievementList[achievementIndex].Achievements.AchievementList)
            {
                if(achievement.CollectableType != AchievementInfoSO.CollectableEnumType.Achievement)
                {
                    if (achievement.IsUnlocked)
                    {
                        _intAmount++;
                    }
                }
            }
        }
    }
    private void UpdateAchievementObject(int objectIndex, int achievementIndex, bool isHidden)
    {
        if (isHidden)
        {
            _achievementObjects[objectIndex].SetIcon(_hiddenAchievement);
            _achievementObjects[objectIndex].SetTitle(_hiddenText);
            _achievementObjects[objectIndex].SetDescription(_hiddenText);
            _achievementObjects[objectIndex].ProgressDisplay(false, _achievementListReference.AchievementList[achievementIndex].CurrentIntAmount, _achievementListReference.AchievementList[achievementIndex].IntGoal,
            _achievementListReference.AchievementList[achievementIndex].CurrentFloatAmount, _achievementListReference.AchievementList[achievementIndex].FloatGoal);
        }
        else
        {
            _achievementObjects[objectIndex].SetIcon(_achievementListReference.AchievementList[achievementIndex].Icon);
            _achievementObjects[objectIndex].SetTitle(_achievementListReference.AchievementList[achievementIndex].Title);
            _achievementObjects[objectIndex].SetDescription(_achievementListReference.AchievementList[achievementIndex].Description);
            if (_achievementListReference.AchievementList[achievementIndex].ShowProgression)
            {
                if (_achievementListReference.AchievementList[achievementIndex].ManualGoalAmount)
                {
                    _achievementObjects[objectIndex].ProgressDisplay(true, _intAmount, _achievementListReference.AchievementList[achievementIndex].IntGoal,
                    _achievementListReference.AchievementList[achievementIndex].CurrentFloatAmount, _achievementListReference.AchievementList[achievementIndex].FloatGoal);
                }
                else
                {
                    if (_achievementListReference.AchievementList[achievementIndex].CollectableType == AchievementInfoSO.CollectableEnumType.Collectable)
                    {
                        _achievementObjects[objectIndex].ProgressDisplay(true, _intAmount, _achievementListReference.AchievementList[achievementIndex].CollectableList.CollectablesList.Count,
                        _achievementListReference.AchievementList[achievementIndex].CurrentFloatAmount, _achievementListReference.AchievementList[achievementIndex].FloatGoal);
                    }
                    else if (_achievementListReference.AchievementList[achievementIndex].CollectableType == AchievementInfoSO.CollectableEnumType.Achievement)
                    {
                        _achievementObjects[objectIndex].ProgressDisplay(true, _intAmount, _achievementListReference.AchievementList[achievementIndex].AchievementCount,
                        _achievementListReference.AchievementList[achievementIndex].CurrentFloatAmount, _achievementListReference.AchievementList[achievementIndex].FloatGoal);
                    }
                }
            }
            else
            {
                _achievementObjects[objectIndex].ProgressDisplay(false, _intAmount, _achievementListReference.AchievementList[achievementIndex].IntGoal,
                _achievementListReference.AchievementList[achievementIndex].CurrentFloatAmount, _achievementListReference.AchievementList[achievementIndex].FloatGoal);
            }
        }
    }
    private void UnlockAchievement(AchievementInfoSO achievement)
    {
        achievement.AchievementUnlocked = true;
        _saveGameEvent.Invoke();
        UpdateUnlockedStatus();
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
            foreach (AchievementInfoSO achievement in _achievementListReference.AchievementList)
            {
                data.TotalAchievementsData.TryGetValue(achievement.AchievementId, out bool isUnlocked);
                achievement.AchievementUnlocked = isUnlocked;
            }
        }
        else
        {
            List<AchievementInfoSO>.Enumerator enumAchievementsList = _achievementListReference.AchievementList.GetEnumerator();
            try
            {
                while (enumAchievementsList.MoveNext())
                {
                    string id = enumAchievementsList.Current.AchievementId;
                    bool value = enumAchievementsList.Current.IsUnlocked;
                    if (data.TotalAchievementsData.ContainsKey(id))
                    {
                        data.TotalAchievementsData.Remove(id);
                    }
                    data.TotalAchievementsData.Add(id, value);
                }
            }
            finally
            {
                enumAchievementsList.Dispose();
            }
        }
        UpdateUnlockedStatus();
        _updateProgressionEvent.Invoke(data);
    }
}