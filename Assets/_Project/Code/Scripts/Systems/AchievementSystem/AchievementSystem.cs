using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AchievementSystem : MonoBehaviour
{
    [SerializeField] private UpdateAchievementsEvent _updateAchievementsEvent;
    [SerializeField] private PlayPopUpDisplayStatusEvent _playPopUpDisplayStatusEvent;
    [SerializeField] private SetAchievementPopUpInfoEvent _setAchievementPopUpInfoEvent;
    [SerializeField] private GenericEmptyEvent _saveGameEvent;
    [SerializeField] private UpdateProgressionEvent _updateProgressionEvent;
    [SerializeField] private AchievementReferenceHolderSO _overAchieverReference;
    [SerializeField] private AchievementListSO _achievementManagerSO;
    [SerializeField] private Sprite _hiddenAchievement;
    [SerializeField] private RectTransform _achievementContainerRect;
    [SerializeField] private AchievementObject _achievementPrefabContainer;
    [SerializeField] private int _displayPopupTime = 5;
    private List<AchievementObject> _achievementObjects;
    private List<int> _QueuedAchievements;
    private int _intAmount = 0;
    private readonly string _hiddenText = "??????????????";
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
        foreach(AchievementInfoSO achievement in _achievementManagerSO.AchievementList)
        {
            achievement.AchievementUnlocked = false;
        }
        Invoke("UpdateUnlockedStatus", 0.01f);
    }
    public void CheckValueRequirement(string achievementID, int? intValue, float? floatValue)
    {
        for (int i = 0; i < _achievementManagerSO.AchievementList.Count; i++)
        {
            if (_achievementManagerSO.AchievementList[i] != null)
            {
                if (achievementID == _achievementManagerSO.AchievementList[i].AchievementId && !_achievementManagerSO.AchievementList[i].IsUnlocked)
                {
                    if(intValue != null || floatValue != null)
                    {
                        if(intValue != null)
                        {
                            if (_achievementManagerSO.AchievementList[i].CollectableType == AchievementInfoSO.CollectableEnumType.None)
                            {
                                _achievementManagerSO.AchievementList[i].UpdateCurrentAmount(intValue, null);
                                if (intValue == _achievementManagerSO.AchievementList[i].IntGoal)
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
                            _achievementManagerSO.AchievementList[i].UpdateCurrentAmount(null, floatValue);
                            if (floatValue == _achievementManagerSO.AchievementList[i].FloatGoal)
                            {
                                UnlockAchievement(i);
                            }
                        }
                        UpdateUnlockedStatus();
                    }
                    else
                    {
                        if (_achievementManagerSO.AchievementList[i].CollectableType == AchievementInfoSO.CollectableEnumType.Achievement)
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
        if (_achievementManagerSO.AchievementList[achievementIndex].CollectableType == AchievementInfoSO.CollectableEnumType.Collectable)
        {
            if (_achievementManagerSO.AchievementList[achievementIndex].CollectableRequirementType == AchievementInfoSO.CollectableRequirementEnumType.Single)
            {
                if (!_achievementManagerSO.AchievementList[achievementIndex].Collectable.IsCollected)
                {
                    return;             
                }
                if (!_achievementManagerSO.AchievementList[achievementIndex].RequiresPreviousAchievement)
                {
                    UnlockAchievement(achievementIndex);
                    return;
                }
                if (_achievementManagerSO.AchievementList[achievementIndex].PreviousAchievement.IsUnlocked)
                {
                    AddToQueueDisplay(achievementIndex);
                    return;
                }
            }
            else
            {
                foreach (CollectableTypeSO collectable in _achievementManagerSO.AchievementList[achievementIndex].CollectableList.CollectablesList)
                {
                    if (collectable.IsCollected)
                    {
                        _intAmount++;
                        if (_achievementManagerSO.AchievementList[achievementIndex].ManualGoalAmount)
                        {
                            if (_intAmount == _achievementManagerSO.AchievementList[achievementIndex].IntGoal)
                            {
                                UnlockAchievement(achievementIndex);
                            }
                        }
                        else
                        {
                            if (_intAmount == _achievementManagerSO.AchievementList[achievementIndex].CollectableList.CollectablesList.Count)
                            {
                                UnlockAchievement(achievementIndex);
                            }
                        }
                    }
                }
            }
        }
        else if(_achievementManagerSO.AchievementList[achievementIndex].CollectableType == AchievementInfoSO.CollectableEnumType.Achievement)
        {
            foreach (AchievementInfoSO achievement in _achievementManagerSO.AchievementList[achievementIndex].Achievements.AchievementList)
            {
                if(achievement.CollectableType != AchievementInfoSO.CollectableEnumType.Achievement)
                {
                    if (achievement.IsUnlocked)
                    {
                        _intAmount++;
                        if (_intAmount == _achievementManagerSO.AchievementList[achievementIndex].AchievementCount)
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
        if (_achievementManagerSO.AchievementList.Count == 0)
        {
            Debug.LogWarning("The list of achievements to unlock is empty!");
            return;
        }
        for (int i = 0; i < _achievementManagerSO.AchievementList.Count; i++)
        {
            if (_achievementManagerSO.AchievementList[i] != null)
            {
                AchievementObject achievementObject = Instantiate(_achievementPrefabContainer, _achievementContainerRect);
                _achievementObjects.Add(achievementObject);
                if (_achievementManagerSO.AchievementList[i].IsHidden)
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
        for (int i = 0; i < _achievementManagerSO.AchievementList.Count; i++)
        {
            if (_achievementManagerSO.AchievementList[i] != null)
            {
                UpdateProgresssionStatus(i);
                if (!_achievementManagerSO.AchievementList[i].IsUnlocked)
                {
                    if (_achievementManagerSO.AchievementList[i].IsHidden)
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
        if (_achievementManagerSO.AchievementList[achievementIndex].CollectableType == AchievementInfoSO.CollectableEnumType.None)
        {
            return;
        }
        else if (_achievementManagerSO.AchievementList[achievementIndex].CollectableType == AchievementInfoSO.CollectableEnumType.Collectable)
        {
            _intAmount = 0;
            if (_achievementManagerSO.AchievementList[achievementIndex].CollectableRequirementType == AchievementInfoSO.CollectableRequirementEnumType.Single)
            {
                if (_achievementManagerSO.AchievementList[achievementIndex].Collectable.IsCollected)
                {
                    _intAmount++;
                }
            }
            else
            {
                foreach (CollectableTypeSO collectable in _achievementManagerSO.AchievementList[achievementIndex].CollectableList.CollectablesList)
                {
                    if (collectable.IsCollected)
                    {
                        _intAmount++;
                    }
                }
            }
        }
        else if (_achievementManagerSO.AchievementList[achievementIndex].CollectableType == AchievementInfoSO.CollectableEnumType.Achievement)
        {
            _intAmount = 0;
            foreach (AchievementInfoSO achievement in _achievementManagerSO.AchievementList[achievementIndex].Achievements.AchievementList)
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
            _achievementObjects[objectIndex].ProgressDisplay(false, _achievementManagerSO.AchievementList[achievementIndex].CurrentIntAmount, _achievementManagerSO.AchievementList[achievementIndex].IntGoal,
            _achievementManagerSO.AchievementList[achievementIndex].CurrentFloatAmount, _achievementManagerSO.AchievementList[achievementIndex].FloatGoal);
        }
        else
        {
            _achievementObjects[objectIndex].SetIcon(_achievementManagerSO.AchievementList[achievementIndex].Icon);
            _achievementObjects[objectIndex].SetTitle(_achievementManagerSO.AchievementList[achievementIndex].Title);
            _achievementObjects[objectIndex].SetDescription(_achievementManagerSO.AchievementList[achievementIndex].Description);
            if (_achievementManagerSO.AchievementList[achievementIndex].ShowProgression)
            {
                if (_achievementManagerSO.AchievementList[achievementIndex].ManualGoalAmount)
                {
                    _achievementObjects[objectIndex].ProgressDisplay(true, _intAmount, _achievementManagerSO.AchievementList[achievementIndex].IntGoal,
                    _achievementManagerSO.AchievementList[achievementIndex].CurrentFloatAmount, _achievementManagerSO.AchievementList[achievementIndex].FloatGoal);
                }
                else
                {
                    if (_achievementManagerSO.AchievementList[achievementIndex].CollectableType == AchievementInfoSO.CollectableEnumType.Collectable)
                    {
                        _achievementObjects[objectIndex].ProgressDisplay(true, _intAmount, _achievementManagerSO.AchievementList[achievementIndex].CollectableList.CollectablesList.Count,
                        _achievementManagerSO.AchievementList[achievementIndex].CurrentFloatAmount, _achievementManagerSO.AchievementList[achievementIndex].FloatGoal);
                    }
                    else if (_achievementManagerSO.AchievementList[achievementIndex].CollectableType == AchievementInfoSO.CollectableEnumType.Achievement)
                    {
                        _achievementObjects[objectIndex].ProgressDisplay(true, _intAmount, _achievementManagerSO.AchievementList[achievementIndex].AchievementCount,
                        _achievementManagerSO.AchievementList[achievementIndex].CurrentFloatAmount, _achievementManagerSO.AchievementList[achievementIndex].FloatGoal);
                    }
                }
            }
            else
            {
                _achievementObjects[objectIndex].ProgressDisplay(false, _intAmount, _achievementManagerSO.AchievementList[achievementIndex].IntGoal,
                _achievementManagerSO.AchievementList[achievementIndex].CurrentFloatAmount, _achievementManagerSO.AchievementList[achievementIndex].FloatGoal);
            }
        }
    }
    private void UnlockAchievement(int achievementID)
    {
        _achievementManagerSO.AchievementList[achievementID].AchievementUnlocked = true;
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
        _updateAchievementsEvent.Invoke(_overAchieverReference.AchievementId, null, null);
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
        _setAchievementPopUpInfoEvent.Invoke(_achievementManagerSO.AchievementList[achievementID].Icon, _achievementManagerSO.AchievementList[achievementID].Title);
        _playPopUpDisplayStatusEvent.Invoke("Displaying");
        _soundEffect = RuntimeManager.CreateInstance(_achievementManagerSO.AchievementList[achievementID].SoundEffect);
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
            foreach (AchievementInfoSO achievement in _achievementManagerSO.AchievementList)
            {
                data.TotalAchievementsData.TryGetValue(achievement.AchievementId, out bool isUnlocked);
                achievement.AchievementUnlocked = isUnlocked;
            }
            UpdateUnlockedStatus();
        }
        else
        {
            List<AchievementInfoSO>.Enumerator enumAchievementsList = _achievementManagerSO.AchievementList.GetEnumerator();
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