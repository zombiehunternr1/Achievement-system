using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AchievementManager : MonoBehaviour
{
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
    public void CheckValueRequirement(string achievementID, int? intValue, float? floatValue)
    {
        for (int i = 0; i < _achievementContainerSO.AchievementList.Count; i++)
        {
            if (_achievementContainerSO.AchievementList[i] != null)
            {
                if (achievementID == _achievementContainerSO.AchievementList[i].AchievementId && !_achievementContainerSO.AchievementList[i].IsUnlocked)
                {
                    if (intValue != null)
                    {
                        if (_achievementContainerSO.AchievementList[i].collectableType == AchievementInfoSO.CollectableType.None)
                        {
                            _achievementContainerSO.AchievementList[i].CurrentIntAmount = (int)intValue;
                            _intAmount = (int)intValue;
                            if (intValue == _achievementContainerSO.AchievementList[i].IntGoal)
                            {
                                UnlockAchievement(i);
                            }
                        }
                        else
                        {
                            _intAmount = 0;
                            foreach (CollectableType collectable in _achievementContainerSO.AchievementList[i].Collectable.CollectablesList)
                            {
                                if (collectable.IsCollected)
                                {
                                    _intAmount++;
                                    if (_intAmount == _achievementContainerSO.AchievementList[i].IntGoal)
                                    {
                                        UnlockAchievement(i);
                                    }
                                }
                            }
                        }
                        UpdateUnlockedStatus();
                    }
                    else if (floatValue != null)
                    {
                        _achievementContainerSO.AchievementList[i].CurrentFloatAmount = (float)floatValue;
                        if (floatValue == _achievementContainerSO.AchievementList[i].FloatGoal)
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
    private void SetupAchievementDisplay()
    {
        if (_achievementContainerSO.AchievementList.Count == 0)
        {
            Debug.LogWarning("The list of achievements to unlock is empty!");
            return;
        }
        for (int i = 0; i < _achievementContainerSO.AchievementList.Count; i++)
        {
            if (_achievementContainerSO.AchievementList[i] != null)
            {
                AchievementObject achievementObject = Instantiate(_achievementPrefabContainer, _achievementContainerRect);
                _achievementObjects.Add(achievementObject);
                if (_achievementContainerSO.AchievementList[i].IsHidden)
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
        for (int i = 0; i < _achievementContainerSO.AchievementList.Count; i++)
        {
            if (_achievementContainerSO.AchievementList[i] != null)
            {
                UpdateProgresssionStatus(i);
                if (!_achievementContainerSO.AchievementList[i].IsUnlocked)
                {
                    if (_achievementContainerSO.AchievementList[i].IsHidden)
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
        if (_achievementContainerSO.AchievementList[achievementIndex].collectableType == AchievementInfoSO.CollectableType.None)
        {
            return;
        }
        _intAmount = 0;
        foreach (CollectableType collectable in _achievementContainerSO.AchievementList[achievementIndex].Collectable.CollectablesList)
        {
            if (collectable.IsCollected)
            {
                _intAmount++;
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
            _achievementObjects[objectIndex].ProgressDisplay(false, _achievementContainerSO.AchievementList[achievementIndex].CurrentIntAmount, _achievementContainerSO.AchievementList[achievementIndex].IntGoal,
            _achievementContainerSO.AchievementList[achievementIndex].CurrentFloatAmount, _achievementContainerSO.AchievementList[achievementIndex].FloatGoal);
        }
        else
        {
            _achievementObjects[objectIndex].SetIcon(_achievementContainerSO.AchievementList[achievementIndex].Icon);
            _achievementObjects[objectIndex].SetTitle(_achievementContainerSO.AchievementList[achievementIndex].Title);
            _achievementObjects[objectIndex].SetDescription(_achievementContainerSO.AchievementList[achievementIndex].Description);
            if (_achievementContainerSO.AchievementList[achievementIndex].ShowProgression)
            {
                _achievementObjects[objectIndex].ProgressDisplay(true, _intAmount, _achievementContainerSO.AchievementList[achievementIndex].IntGoal,
                _achievementContainerSO.AchievementList[achievementIndex].CurrentFloatAmount, _achievementContainerSO.AchievementList[achievementIndex].FloatGoal);
            }
            else
            {
                _achievementObjects[objectIndex].ProgressDisplay(false, _intAmount, _achievementContainerSO.AchievementList[achievementIndex].IntGoal,
                _achievementContainerSO.AchievementList[achievementIndex].CurrentFloatAmount, _achievementContainerSO.AchievementList[achievementIndex].FloatGoal);
            }
        }
    }
    private void UnlockAchievement(int achievementID)
    {
        _achievementContainerSO.AchievementList[achievementID].AchievementUnlocked = true;
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
        _achievementPrefabPopup.SetIcon(_achievementContainerSO.AchievementList[achievementID].Icon);
        _achievementPrefabPopup.SetTitle(_achievementContainerSO.AchievementList[achievementID].Title);
        _achievementPrefabPopup.PlayDisplayAnim();
        _soundEffect = RuntimeManager.CreateInstance(_achievementContainerSO.AchievementList[achievementID].SoundEffect);
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
            foreach (AchievementInfoSO achievement in _achievementContainerSO.AchievementList)
            {
                data.TotalAchievementsData.TryGetValue(achievement.AchievementId, out bool isUnlocked);
                achievement.AchievementUnlocked = isUnlocked;
            }
            UpdateUnlockedStatus();
        }
        else
        {
            int currentIndexCount = 0;
            while (currentIndexCount < _achievementContainerSO.AchievementList.Count)
            {
                if (data.TotalAchievementsData.ContainsKey(_achievementContainerSO.AchievementList[currentIndexCount].AchievementId))
                {
                    data.TotalAchievementsData.Remove(_achievementContainerSO.AchievementList[currentIndexCount].AchievementId);
                }
                data.TotalAchievementsData.Add(_achievementContainerSO.AchievementList[currentIndexCount].AchievementId, _achievementContainerSO.AchievementList[currentIndexCount].IsUnlocked);
                Debug.Log(_achievementContainerSO.AchievementList[currentIndexCount]);
                currentIndexCount++;
            }
        }
        _updateProgressionEvent.Invoke(data);
    }
}