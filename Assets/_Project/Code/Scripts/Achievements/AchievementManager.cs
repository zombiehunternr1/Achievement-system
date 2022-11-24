using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AchievementManager : MonoBehaviour
{
    [SerializeField] private UpdateAchievementsEvent _updateAchievementsEvent;
    [SerializeField] private GenericEmptyEvent _saveGameEvent;
    [SerializeField] private UpdateProgressionEvent _updateProgressionEvent;
    [SerializeField] private AchievementReferenceHolderSO _overAchieverReference;
    [SerializeField] private AchievementListSO _achievementManagerSO;
    [SerializeField] private Sprite _hiddenAchievement;
    [SerializeField] private RectTransform _achievementContainerRect;
    [SerializeField] private AchievementObject _achievementPrefabContainer;
    [SerializeField] private AchievementObject _achievementPrefabPopup;
    [SerializeField] private int _displayPopupTime = 5;
    private List<AchievementObject> _achievementObjects;
    private List<int> _QueuedAchievements;
    private int _intAmount = 0;
    private string _hiddenText = "??????????????";
    private EventInstance _soundEffect;
    private void Awake()
    {
        _achievementObjects = new List<AchievementObject>();
        _QueuedAchievements = new List<int>();
        SetupAchievementDisplay();
    }
    public void StartPopupCooldown()
    {
        StartCoroutine(PopupCooldown());
    }
    public void ResetAllAchievements()
    {
        foreach(AchievementInfoSO achievement in _achievementManagerSO.achievementList)
        {
            achievement.achievementUnlocked = false;
        }
        Invoke("UpdateUnlockedStatus", 0.01f);
    }
    public void CheckValueRequirement(string achievementID, int? intValue, float? floatValue)
    {
        for (int i = 0; i < _achievementManagerSO.achievementList.Count; i++)
        {
            if (_achievementManagerSO.achievementList[i] != null)
            {
                if (achievementID == _achievementManagerSO.achievementList[i].achievementId && !_achievementManagerSO.achievementList[i].isUnlocked)
                {
                    if(intValue != null || floatValue != null)
                    {
                        if(intValue != null)
                        {
                            if (_achievementManagerSO.achievementList[i].collectableType == AchievementInfoSO.CollectableType.none)
                            {
                                _achievementManagerSO.achievementList[i].currentIntAmount = (int)intValue;
                                if (intValue == _achievementManagerSO.achievementList[i].intGoal)
                                {
                                    UnlockAchievement(i);
                                }
                            }
                            else
                            {
                                CheckCollectables(i);
                            }
                        }
                        else if(floatValue != null)
                        {
                            _achievementManagerSO.achievementList[i].currentFloatAmount = (float)floatValue;
                            if (floatValue == _achievementManagerSO.achievementList[i].floatGoal)
                            {
                                UnlockAchievement(i);
                            }
                        }
                        UpdateUnlockedStatus();
                    }
                    else
                    {
                        if (_achievementManagerSO.achievementList[i].collectableType == AchievementInfoSO.CollectableType.achievement)
                        {
                            CheckCollectables(i);
                        }
                        else
                        {
                            UnlockAchievement(i);
                        }
                    }
                }
            }
        }
    }
    private void CheckCollectables(int achievementIndex)
    {
        _intAmount = 0;
        if (_achievementManagerSO.achievementList[achievementIndex].collectableType == AchievementInfoSO.CollectableType.collectable)
        {
            if (_achievementManagerSO.achievementList[achievementIndex].collectableRequirementType == AchievementInfoSO.CollectableRequirementType.single)
            {
                if (!_achievementManagerSO.achievementList[achievementIndex].collectable.isCollected)
                {
                    return;             
                }
                if (!_achievementManagerSO.achievementList[achievementIndex].requiresPreviousAchievement)
                {
                    UnlockAchievement(achievementIndex);
                    return;
                }
                if (_achievementManagerSO.achievementList[achievementIndex].previousAchievement.isUnlocked)
                {
                    AddToQueueDisplay(achievementIndex);
                    return;
                }
            }
            else
            {
                foreach (CollectableTypeSO collectable in _achievementManagerSO.achievementList[achievementIndex].collectableList.collectablesList)
                {
                    if (collectable.isCollected)
                    {
                        _intAmount++;
                        if (_achievementManagerSO.achievementList[achievementIndex].manualGoalAmount)
                        {
                            if (_intAmount == _achievementManagerSO.achievementList[achievementIndex].intGoal)
                            {
                                UnlockAchievement(achievementIndex);
                            }
                        }
                        else
                        {
                            if (_intAmount == _achievementManagerSO.achievementList[achievementIndex].collectableList.collectablesList.Count)
                            {
                                UnlockAchievement(achievementIndex);
                            }
                        }
                    }
                }
            }
        }
        else if(_achievementManagerSO.achievementList[achievementIndex].collectableType == AchievementInfoSO.CollectableType.achievement)
        {
            foreach (AchievementInfoSO achievement in _achievementManagerSO.achievementList[achievementIndex].achievements.achievementList)
            {
                if(achievement.collectableType != AchievementInfoSO.CollectableType.achievement)
                {
                    if (achievement.isUnlocked)
                    {
                        _intAmount++;
                        if (_intAmount == _achievementManagerSO.achievementList[achievementIndex].achievementCount)
                        {
                            UnlockAchievement(achievementIndex);
                        }
                    }
                }
            }
        }
    }
    private void SetupAchievementDisplay()
    {
        if (_achievementManagerSO.achievementList.Count == 0)
        {
            Debug.LogWarning("The list of achievements to unlock is empty!");
            return;
        }
        for (int i = 0; i < _achievementManagerSO.achievementList.Count; i++)
        {
            if (_achievementManagerSO.achievementList[i] != null)
            {
                AchievementObject achievementObject = Instantiate(_achievementPrefabContainer, _achievementContainerRect);
                _achievementObjects.Add(achievementObject);
                if (_achievementManagerSO.achievementList[i].isHidden)
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
    private void UpdateUnlockedStatus()
    {
        int objectIndex = 0;
        for (int i = 0; i < _achievementManagerSO.achievementList.Count; i++)
        {
            if (_achievementManagerSO.achievementList[i] != null)
            {
                UpdateProgresssionStatus(i);
                if (!_achievementManagerSO.achievementList[i].isUnlocked)
                {
                    if (_achievementManagerSO.achievementList[i].isHidden)
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
        if (_achievementManagerSO.achievementList[achievementIndex].collectableType == AchievementInfoSO.CollectableType.none)
        {
            return;
        }
        else if (_achievementManagerSO.achievementList[achievementIndex].collectableType == AchievementInfoSO.CollectableType.collectable)
        {
            _intAmount = 0;
            if (_achievementManagerSO.achievementList[achievementIndex].collectableRequirementType == AchievementInfoSO.CollectableRequirementType.single)
            {
                if (_achievementManagerSO.achievementList[achievementIndex].collectable.isCollected)
                {
                    _intAmount++;
                }
            }
            else
            {
                foreach (CollectableTypeSO collectable in _achievementManagerSO.achievementList[achievementIndex].collectableList.collectablesList)
                {
                    if (collectable.isCollected)
                    {
                        _intAmount++;
                    }
                }
            }
        }
        else if (_achievementManagerSO.achievementList[achievementIndex].collectableType == AchievementInfoSO.CollectableType.achievement)
        {
            _intAmount = 0;
            foreach (AchievementInfoSO achievement in _achievementManagerSO.achievementList[achievementIndex].achievements.achievementList)
            {
                if(achievement.collectableType != AchievementInfoSO.CollectableType.achievement)
                {
                    if (achievement.isUnlocked)
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
            _achievementObjects[objectIndex].ProgressDisplay(false, _achievementManagerSO.achievementList[achievementIndex].currentIntAmount, _achievementManagerSO.achievementList[achievementIndex].intGoal,
            _achievementManagerSO.achievementList[achievementIndex].currentFloatAmount, _achievementManagerSO.achievementList[achievementIndex].floatGoal);
        }
        else
        {
            _achievementObjects[objectIndex].SetIcon(_achievementManagerSO.achievementList[achievementIndex].icon);
            _achievementObjects[objectIndex].SetTitle(_achievementManagerSO.achievementList[achievementIndex].title);
            _achievementObjects[objectIndex].SetDescription(_achievementManagerSO.achievementList[achievementIndex].description);
            if (_achievementManagerSO.achievementList[achievementIndex].showProgression)
            {
                if (_achievementManagerSO.achievementList[achievementIndex].manualGoalAmount)
                {
                    _achievementObjects[objectIndex].ProgressDisplay(true, _intAmount, _achievementManagerSO.achievementList[achievementIndex].intGoal,
                    _achievementManagerSO.achievementList[achievementIndex].currentFloatAmount, _achievementManagerSO.achievementList[achievementIndex].floatGoal);
                }
                else
                {
                    if (_achievementManagerSO.achievementList[achievementIndex].collectableType == AchievementInfoSO.CollectableType.collectable)
                    {
                        _achievementObjects[objectIndex].ProgressDisplay(true, _intAmount, _achievementManagerSO.achievementList[achievementIndex].collectableList.collectablesList.Count,
                        _achievementManagerSO.achievementList[achievementIndex].currentFloatAmount, _achievementManagerSO.achievementList[achievementIndex].floatGoal);
                    }
                    else if (_achievementManagerSO.achievementList[achievementIndex].collectableType == AchievementInfoSO.CollectableType.achievement)
                    {
                        _achievementObjects[objectIndex].ProgressDisplay(true, _intAmount, _achievementManagerSO.achievementList[achievementIndex].achievementCount,
                        _achievementManagerSO.achievementList[achievementIndex].currentFloatAmount, _achievementManagerSO.achievementList[achievementIndex].floatGoal);
                    }
                }
            }
            else
            {
                _achievementObjects[objectIndex].ProgressDisplay(false, _intAmount, _achievementManagerSO.achievementList[achievementIndex].intGoal,
                _achievementManagerSO.achievementList[achievementIndex].currentFloatAmount, _achievementManagerSO.achievementList[achievementIndex].floatGoal);
            }
        }
    }
    private void UnlockAchievement(int achievementID)
    {
        _achievementManagerSO.achievementList[achievementID].achievementUnlocked = true;
        _saveGameEvent.Invoke();
        UpdateUnlockedStatus();
        AddToQueueDisplay(achievementID);
    }
    private void AddToQueueDisplay(int achievementID)
    {
        if (_QueuedAchievements.Count == 0)
        {
            _QueuedAchievements.Add(achievementID);
            DisplayPopUpAchievement(achievementID);
        }
        else
        {
            _QueuedAchievements.Add(achievementID);
        }
        _updateAchievementsEvent.Invoke(_overAchieverReference.achievementId, null, null);
    }
    private void DisplayNextinQueue()
    {
        _QueuedAchievements.RemoveAt(0);
        if (_QueuedAchievements.Count != 0)
        {
            DisplayPopUpAchievement(_QueuedAchievements[0]);
        }
    }
    private void DisplayPopUpAchievement(int achievementID)
    {
        _achievementPrefabPopup.SetIcon(_achievementManagerSO.achievementList[achievementID].icon);
        _achievementPrefabPopup.SetTitle(_achievementManagerSO.achievementList[achievementID].title);
        _achievementPrefabPopup.PlayDisplayAnim();
        _soundEffect = RuntimeManager.CreateInstance(_achievementManagerSO.achievementList[achievementID].soundEffect);
        RuntimeManager.AttachInstanceToGameObject(_soundEffect, transform);
        _soundEffect.start();
        _soundEffect.release();
    }
    private IEnumerator PopupCooldown()
    {
        yield return new WaitForSeconds(_displayPopupTime);
        _achievementPrefabPopup.PlayHideAnim();
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
            foreach (AchievementInfoSO achievement in _achievementManagerSO.achievementList)
            {
                data.totalAchievementsData.TryGetValue(achievement.achievementId, out bool isUnlocked);
                achievement.achievementUnlocked = isUnlocked;
            }
            UpdateUnlockedStatus();
        }
        else
        {
            List<AchievementInfoSO>.Enumerator enumAchievementsList = _achievementManagerSO.achievementList.GetEnumerator();
            try
            {
                while (enumAchievementsList.MoveNext())
                {
                    string id = enumAchievementsList.Current.achievementId;
                    bool value = enumAchievementsList.Current.isUnlocked;
                    if (data.totalAchievementsData.ContainsKey(id))
                    {
                        data.totalAchievementsData.Remove(id);
                    }
                    data.totalAchievementsData.Add(id, value);
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