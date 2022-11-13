using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AchievementManager : MonoBehaviour
{
    [SerializeField] private AchievementEvent _overAchieverEvent;
    [SerializeField] private AchievementContainerSO _achievementContainerSO;
    [SerializeField] private GenericEmptyEvent _saveGameEvent;
    [SerializeField] private UpdateProgressionEvent _updateProgressionEvent;
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
        foreach(AchievementInfoSO achievement in _achievementContainerSO.achievementList)
        {
            achievement.achievementUnlocked = false;
        }
        Invoke("UpdateUnlockedStatus", 0.01f);
    }
    public void CheckValueRequirement(string achievementID, int? intValue, float? floatValue)
    {
        for (int i = 0; i < _achievementContainerSO.achievementList.Count; i++)
        {
            if (_achievementContainerSO.achievementList[i] != null)
            {
                if (achievementID == _achievementContainerSO.achievementList[i].achievementId && !_achievementContainerSO.achievementList[i].isUnlocked)
                {
                    if (intValue != null)
                    {
                        if (_achievementContainerSO.achievementList[i].collectableType == AchievementInfoSO.CollectableType.None)
                        {
                            _achievementContainerSO.achievementList[i].currentIntAmount = (int)intValue;
                            _intAmount = (int)intValue;
                            if (intValue == _achievementContainerSO.achievementList[i].intGoal)
                            {
                                UnlockAchievement(i);
                            }
                        }
                        else
                        {
                            CheckCollectables(i);
                        }
                        UpdateUnlockedStatus();
                    }
                    else if (floatValue != null)
                    {
                        _achievementContainerSO.achievementList[i].currentFloatAmount = (float)floatValue;
                        if (floatValue == _achievementContainerSO.achievementList[i].floatGoal)
                        {
                            UnlockAchievement(i);
                            return;
                        }
                        UpdateUnlockedStatus();
                    }
                    else
                    {
                        UnlockAchievement(i);
                    }
                }
            }
        }
    }
    private void CheckCollectables(int achievementIndex)
    {
        _intAmount = 0;
        if (_achievementContainerSO.achievementList[achievementIndex].collectableType == AchievementInfoSO.CollectableType.Collectable)
        {
            foreach (CollectableType collectable in _achievementContainerSO.achievementList[achievementIndex].collectable.collectablesList)
            {
                if (collectable.isCollected)
                {
                    _intAmount++;
                    if (_achievementContainerSO.achievementList[achievementIndex].manualGoalAmount)
                    {
                        if (_intAmount == _achievementContainerSO.achievementList[achievementIndex].intGoal)
                        {
                            UnlockAchievement(achievementIndex);
                        }
                    }
                    else
                    {
                        if (_intAmount == _achievementContainerSO.achievementList[achievementIndex].collectable.collectablesList.Count)
                        {
                            UnlockAchievement(achievementIndex);
                        }
                    }
                }
            }
        }
        else if(_achievementContainerSO.achievementList[achievementIndex].collectableType == AchievementInfoSO.CollectableType.Achievement)
        {
            foreach (AchievementInfoSO achievement in _achievementContainerSO.achievementList[achievementIndex].achievements.achievementList)
            {
                if (achievement.isUnlocked)
                {
                    _intAmount++;
                    if (_intAmount == _achievementContainerSO.achievementList[achievementIndex].achievements.achievementList.Count)
                    {
                        UnlockAchievement(achievementIndex);
                    }
                }
            }
        }
    }

    private void SetupAchievementDisplay()
    {
        if (_achievementContainerSO.achievementList.Count == 0)
        {
            Debug.LogWarning("The list of achievements to unlock is empty!");
            return;
        }
        for (int i = 0; i < _achievementContainerSO.achievementList.Count; i++)
        {
            if (_achievementContainerSO.achievementList[i] != null)
            {
                AchievementObject achievementObject = Instantiate(_achievementPrefabContainer, _achievementContainerRect);
                _achievementObjects.Add(achievementObject);
                if (_achievementContainerSO.achievementList[i].isHidden)
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
        for (int i = 0; i < _achievementContainerSO.achievementList.Count; i++)
        {
            if (_achievementContainerSO.achievementList[i] != null)
            {
                UpdateProgresssionStatus(i);
                if (!_achievementContainerSO.achievementList[i].isUnlocked)
                {
                    if (_achievementContainerSO.achievementList[i].isHidden)
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
        if (_achievementContainerSO.achievementList[achievementIndex].collectableType == AchievementInfoSO.CollectableType.None)
        {
            return;
        }
        else if (_achievementContainerSO.achievementList[achievementIndex].collectableType == AchievementInfoSO.CollectableType.Collectable)
        {
            _intAmount = 0;
            foreach (CollectableType collectable in _achievementContainerSO.achievementList[achievementIndex].collectable.collectablesList)
            {
                if (collectable.isCollected)
                {
                    _intAmount++;
                }
            }
        }
        else if (_achievementContainerSO.achievementList[achievementIndex].collectableType == AchievementInfoSO.CollectableType.Achievement)
        {
            _intAmount = 0;
            foreach (AchievementInfoSO achievement in _achievementContainerSO.achievementList[achievementIndex].achievements.achievementList)
            {
                if (achievement.isUnlocked)
                {
                    _intAmount++;
                }
            }
            Debug.Log(_intAmount);
        }
    }
    private void UpdateAchievementObject(int objectIndex, int achievementIndex, bool isHidden)
    {
        if (isHidden)
        {
            _achievementObjects[objectIndex].SetIcon(_hiddenAchievement);
            _achievementObjects[objectIndex].SetTitle(_hiddenText);
            _achievementObjects[objectIndex].SetDescription(_hiddenText);
            _achievementObjects[objectIndex].ProgressDisplay(false, _achievementContainerSO.achievementList[achievementIndex].currentIntAmount, _achievementContainerSO.achievementList[achievementIndex].intGoal,
            _achievementContainerSO.achievementList[achievementIndex].currentFloatAmount, _achievementContainerSO.achievementList[achievementIndex].floatGoal);
        }
        else
        {
            _achievementObjects[objectIndex].SetIcon(_achievementContainerSO.achievementList[achievementIndex].icon);
            _achievementObjects[objectIndex].SetTitle(_achievementContainerSO.achievementList[achievementIndex].title);
            _achievementObjects[objectIndex].SetDescription(_achievementContainerSO.achievementList[achievementIndex].description);
            if (_achievementContainerSO.achievementList[achievementIndex].showProgression)
            {
                _achievementObjects[objectIndex].ProgressDisplay(true, _intAmount, _achievementContainerSO.achievementList[achievementIndex].intGoal,
                _achievementContainerSO.achievementList[achievementIndex].currentFloatAmount, _achievementContainerSO.achievementList[achievementIndex].floatGoal);
            }
            else
            {
                _achievementObjects[objectIndex].ProgressDisplay(false, _intAmount, _achievementContainerSO.achievementList[achievementIndex].intGoal,
                _achievementContainerSO.achievementList[achievementIndex].currentFloatAmount, _achievementContainerSO.achievementList[achievementIndex].floatGoal);
            }
        }
    }
    private void UnlockAchievement(int achievementID)
    {
        _achievementContainerSO.achievementList[achievementID].achievementUnlocked = true;
        _saveGameEvent.Invoke();
        _overAchieverEvent.Invoke(_overAchieverEvent.AchievementID, null, null);
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
        _achievementPrefabPopup.SetIcon(_achievementContainerSO.achievementList[achievementID].icon);
        _achievementPrefabPopup.SetTitle(_achievementContainerSO.achievementList[achievementID].title);
        _achievementPrefabPopup.PlayDisplayAnim();
        _soundEffect = RuntimeManager.CreateInstance(_achievementContainerSO.achievementList[achievementID].soundEffect);
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
            foreach (AchievementInfoSO achievement in _achievementContainerSO.achievementList)
            {
                data.totalAchievementsData.TryGetValue(achievement.achievementId, out bool isUnlocked);
                achievement.achievementUnlocked = isUnlocked;
            }
            UpdateUnlockedStatus();
        }
        else
        {
            List<AchievementInfoSO>.Enumerator enumAchievementsList = _achievementContainerSO.achievementList.GetEnumerator();
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
        _updateProgressionEvent.Invoke(data);
    }
}