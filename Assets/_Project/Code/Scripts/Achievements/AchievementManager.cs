using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AchievementManager : MonoBehaviour
{
    [SerializeField] private AchievementManagerSO _achievementManager;
    [SerializeField] private Sprite _hiddenAchievement;
    [SerializeField] private RectTransform _achievementContainer;
    [SerializeField] private AchievementObject _achievementPrefabContainer;
    [SerializeField] private AchievementObject _achievementPrefabPopup;
    [SerializeField] private int _displayPopupTime = 5;
    private List<AchievementObject> _achievementObjects;
    private List<int> _QueuedAchievements;
    private string _hiddenText = "??????????????";
    private EventInstance _soundEffect;
    private void OnEnable()
    {
        _achievementObjects = new List<AchievementObject>();
        _QueuedAchievements = new List<int>();
        SetupAchievementDisplay();
    }
    public void StartPopupCooldown()
    {
        StartCoroutine(PopupCooldown());
    }
    public void CheckValueRequirement(int achievementID, int? intValue, float? floatValue)
    {
        for (int i = 0; i < _achievementManager.AchievementList.Count; i++)
        {
            if (_achievementManager.AchievementList[i] != null)
            {
                if (achievementID == _achievementManager.AchievementList[i].AchievementId && !_achievementManager.AchievementList[i].IsUnlocked)
                {
                    if (intValue != null)
                    {
                        if (intValue == _achievementManager.AchievementList[i].IntGoal)
                        {
                            UnlockAchievement(i);
                            return;
                        }
                    }
                    else if (floatValue != null)
                    {
                        if (floatValue == _achievementManager.AchievementList[i].FloatGoal)
                        {
                            UnlockAchievement(i);
                            return;
                        }
                    }
                    else
                    {
                        UnlockAchievement(i);
                        return;
                    }
                }
            }         
        }
    }
    private void SetupAchievementDisplay()
    {
        if(_achievementManager.AchievementList.Count == 0)
        {
            Debug.LogWarning("The list of achievements to unlock is empty!");
            return;
        }
        for (int i = 0; i < _achievementManager.AchievementList.Count; i++)
        {
            if (_achievementManager.AchievementList[i] != null)
            {
                AchievementObject achievementObject = Instantiate(_achievementPrefabContainer, _achievementContainer);
                _achievementObjects.Add(achievementObject);
                if (_achievementManager.AchievementList[i].IsHidden)
                {
                    achievementObject.setIcon(_hiddenAchievement);
                    achievementObject.setTitle(_hiddenText);
                    achievementObject.setDescription(_hiddenText);
                    achievementObject.RemoveLockIcon();
                }
                else
                {
                    achievementObject.GetComponent<AchievementObject>().setIcon(_achievementManager.AchievementList[i].Icon);
                    achievementObject.GetComponent<AchievementObject>().setTitle(_achievementManager.AchievementList[i].Title);
                    achievementObject.GetComponent<AchievementObject>().setDescription(_achievementManager.AchievementList[i].Description);
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
        for (int i = 0; i < _achievementManager.AchievementList.Count; i++)
        {
            if (_achievementManager.AchievementList[i] != null)
            {
                if (_achievementManager.AchievementList[i].IsUnlocked && !_achievementManager.AchievementList[i].IsHidden)
                {
                    _achievementObjects[objectIndex].UnlockAchievement();
                }
                else if (_achievementManager.AchievementList[i].IsUnlocked && _achievementManager.AchievementList[i].IsHidden)
                {
                    _achievementObjects[objectIndex].setIcon(_achievementManager.AchievementList[i].Icon);
                    _achievementObjects[objectIndex].setTitle(_achievementManager.AchievementList[i].Title);
                    _achievementObjects[objectIndex].setDescription(_achievementManager.AchievementList[i].Description);
                    _achievementObjects[objectIndex].UnlockAchievement();
                }
                objectIndex++;
            }
        }
    }
    private void UnlockAchievement(int achievementID)
    {
        _achievementManager.AchievementList[achievementID].AchievementUnlocked = true;
        //Save to JSON!!
        UpdateUnlockedStatus();
        AddToQueueDisplay(achievementID);
    }
    private void AddToQueueDisplay(int achievementID)
    {
        if(_QueuedAchievements.Count == 0)
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
        if(_QueuedAchievements.Count != 0)
        {
            DisplayPopUpAchievement(_QueuedAchievements[0]);
        }
    }
    private void DisplayPopUpAchievement(int achievementID)
    {
        _achievementPrefabPopup.setIcon(_achievementManager.AchievementList[achievementID].Icon);
        _achievementPrefabPopup.setTitle(_achievementManager.AchievementList[achievementID].Title);
        _achievementPrefabPopup.PlayDisplayAnim();
        _soundEffect = RuntimeManager.CreateInstance(_achievementManager.AchievementList[achievementID].SoundEffect);
        RuntimeManager.AttachInstanceToGameObject(_soundEffect, transform);
        _soundEffect.start();
        _soundEffect.release();
    }
    private IEnumerator PopupCooldown()
    {
        yield return new WaitForSeconds(_displayPopupTime);
        _achievementPrefabPopup.PlayHideAnim();
        yield return new WaitForSeconds(1.5f);
        if(_QueuedAchievements.Count != 0)
        {
            DisplayNextinQueue();
        }
        else
        {
            StopAllCoroutines();
        }
    }
}
