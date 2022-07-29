using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMOD.Studio;
using FMODUnity;

public class AchievementManager : MonoBehaviour
{
    [SerializeField] private Sprite hiddenAchievement;
    [SerializeField] private RectTransform achievementContainer;
    [SerializeField] private AchievementObject achievementPrefabContainer;
    [SerializeField] private AchievementObject achievementPrefabPopup;
    [SerializeField] private int displayPopupTime = 5;
    [SerializeField] private List<AchievementInfo> achievementsTotal;

    private List<AchievementObject> achievementObjects;
    private List<int> QueuedAchievements;
    private string hiddenText = "??????????????";

    private EventInstance soundEffect;

    private void OnEnable()
    {
        achievementObjects = new List<AchievementObject>();
        QueuedAchievements = new List<int>();
        SetupAchievementDisplay();
    }

    private void SetupAchievementDisplay()
    {
        for (int i = 0; i < achievementsTotal.Count; i++)
        {
            AchievementObject achievementObject = Instantiate(achievementPrefabContainer, achievementContainer);
            achievementObjects.Add(achievementObject);
            if (achievementsTotal[i].CheckIfHidden())
            {
                achievementObjects[i].setIcon(hiddenAchievement);
                achievementObjects[i].setTitle(hiddenText);
                achievementObjects[i].setDescription(hiddenText);
                achievementObjects[i].RemoveLockIcon();
            }
            else
            {
                achievementObjects[i].GetComponent<AchievementObject>().setIcon(achievementsTotal[i].GetIcon());
                achievementObjects[i].GetComponent<AchievementObject>().setTitle(achievementsTotal[i].GetTitle());
                achievementObjects[i].GetComponent<AchievementObject>().setDescription(achievementsTotal[i].GetDescription());
            }
        }
        updateUnlockedStatus();
    }

    private void updateUnlockedStatus()
    {
        for(int i = 0; i < achievementsTotal.Count; i++)
        {
            if (achievementsTotal[i].CheckIfUnlocked() && !achievementsTotal[i].CheckIfHidden())
            {
                achievementObjects[i].UnlockAchievement();
            }
            else if (achievementsTotal[i].CheckIfUnlocked() && achievementsTotal[i].CheckIfHidden())
            {
                achievementObjects[i].setIcon(achievementsTotal[i].GetIcon());
                achievementObjects[i].setTitle(achievementsTotal[i].GetTitle());
                achievementObjects[i].setDescription(achievementsTotal[i].GetDescription());
                achievementObjects[i].UnlockAchievement();
            }
        }
    }

    public void CheckValueRequirement(int achievementID, int? intValue, float? floatValue)
    {
        for (int i = 0; i < achievementsTotal.Count; i++)
        {
            if (achievementID == achievementsTotal[i].GetAchievementID() && !achievementsTotal[i].CheckIfUnlocked())
            {
                if (intValue != null)
                {
                    if (intValue == achievementsTotal[i].GetIntGoal())
                    {
                        UnlockAchievement(i);
                        return;
                    }
                }
                else if (floatValue != null)
                {
                    if (floatValue == achievementsTotal[i].GetFloatGoal())
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

    private void UnlockAchievement(int achievementID)
    {
        achievementsTotal[achievementID].UnlockAchievement();
        updateUnlockedStatus();
        AddToQueueDisplay(achievementID);
    }

    private void AddToQueueDisplay(int achievementID)
    {
        if(QueuedAchievements.Count == 0)
        {
            QueuedAchievements.Add(achievementID);
            DisplayPopUpAchievement(achievementID);
        }
        else
        {
            QueuedAchievements.Add(achievementID);
        }
    }

    private void DisplayNextinQueue()
    {
        QueuedAchievements.RemoveAt(0);
        if(QueuedAchievements.Count != 0)
        {
            DisplayPopUpAchievement(QueuedAchievements[0]);
        }
    }

    private void DisplayPopUpAchievement(int achievementID)
    {
        achievementPrefabPopup.setIcon(achievementsTotal[achievementID].GetIcon());
        achievementPrefabPopup.setTitle(achievementsTotal[achievementID].GetTitle());
        achievementPrefabPopup.PlayDisplayAnim();
        soundEffect = RuntimeManager.CreateInstance(achievementsTotal[achievementID].GetSoundEffect());
        RuntimeManager.AttachInstanceToGameObject(soundEffect, transform);
        soundEffect.start();
        soundEffect.release();
    }

    public void StartPopupCooldown()
    {
        StartCoroutine(PopupCooldown());
    }

    private IEnumerator PopupCooldown()
    {
        yield return new WaitForSeconds(displayPopupTime);
        achievementPrefabPopup.PlayHideAnim();
        yield return new WaitForSeconds(1.5f);
        if(QueuedAchievements.Count != 0)
        {
            DisplayNextinQueue();
        }
        else
        {
            StopAllCoroutines();
        }
    }
}
