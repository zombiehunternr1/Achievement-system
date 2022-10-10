using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AchievementManager : MonoBehaviour
{
    [SerializeField] private Sprite _hiddenAchievement;
    [SerializeField] private RectTransform _achievementContainer;
    [SerializeField] private AchievementObject _achievementPrefabContainer;
    [SerializeField] private AchievementObject _achievementPrefabPopup;
    [SerializeField] private int _displayPopupTime = 5;
    [SerializeField] private List<AchievementInfo> _totalAchievementsToUnlock;

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
        for (int i = 0; i < _totalAchievementsToUnlock.Count; i++)
        {
            if (achievementID == _totalAchievementsToUnlock[i].AchievementId && !_totalAchievementsToUnlock[i].IsUnlocked)
            {
                if (intValue != null)
                {
                    if (intValue == _totalAchievementsToUnlock[i].IntGoal)
                    {
                        UnlockAchievement(i);
                        return;
                    }
                }
                else if (floatValue != null)
                {
                    if (floatValue == _totalAchievementsToUnlock[i].FloatGoal)
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
    private void SetupAchievementDisplay()
    {
        for (int i = 0; i < _totalAchievementsToUnlock.Count; i++)
        {
            AchievementObject achievementObject = Instantiate(_achievementPrefabContainer, _achievementContainer);
            _achievementObjects.Add(achievementObject);
            if (_totalAchievementsToUnlock[i].IsHidden)
            {
                _achievementObjects[i].setIcon(_hiddenAchievement);
                _achievementObjects[i].setTitle(_hiddenText);
                _achievementObjects[i].setDescription(_hiddenText);
                _achievementObjects[i].RemoveLockIcon();
            }
            else
            {
                _achievementObjects[i].GetComponent<AchievementObject>().setIcon(_totalAchievementsToUnlock[i].Icon);
                _achievementObjects[i].GetComponent<AchievementObject>().setTitle(_totalAchievementsToUnlock[i].Title);
                _achievementObjects[i].GetComponent<AchievementObject>().setDescription(_totalAchievementsToUnlock[i].Description);
            }
        }
        UpdateUnlockedStatus();
    }

    private void UpdateUnlockedStatus()
    {
        for(int i = 0; i < _totalAchievementsToUnlock.Count; i++)
        {
            if (_totalAchievementsToUnlock[i].IsUnlocked && !_totalAchievementsToUnlock[i].IsHidden)
            {
                _achievementObjects[i].UnlockAchievement();
            }
            else if (_totalAchievementsToUnlock[i].IsUnlocked && _totalAchievementsToUnlock[i].IsHidden)
            {
                _achievementObjects[i].setIcon(_totalAchievementsToUnlock[i].Icon);
                _achievementObjects[i].setTitle(_totalAchievementsToUnlock[i].Title);
                _achievementObjects[i].setDescription(_totalAchievementsToUnlock[i].Description);
                _achievementObjects[i].UnlockAchievement();
            }
        }
    }
    private void UnlockAchievement(int achievementID)
    {
        _totalAchievementsToUnlock[achievementID].AchievementUnlocked = true;
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
        _achievementPrefabPopup.setIcon(_totalAchievementsToUnlock[achievementID].Icon);
        _achievementPrefabPopup.setTitle(_totalAchievementsToUnlock[achievementID].Title);
        _achievementPrefabPopup.PlayDisplayAnim();
        _soundEffect = RuntimeManager.CreateInstance(_totalAchievementsToUnlock[achievementID].SoundEffect);
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
