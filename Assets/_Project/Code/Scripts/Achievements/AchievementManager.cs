using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
            if (achievementsTotal[i].isHidden)
            {
                achievementObjects[i].setIcon(hiddenAchievement);
                achievementObjects[i].setTitle(hiddenText);
                achievementObjects[i].setDescription(hiddenText);
                achievementObjects[i].RemoveLockIcon();
            }
            else
            {
                achievementObjects[i].GetComponent<AchievementObject>().setIcon(achievementsTotal[i].icon);
                achievementObjects[i].GetComponent<AchievementObject>().setTitle(achievementsTotal[i].title);
                achievementObjects[i].GetComponent<AchievementObject>().setDescription(achievementsTotal[i].description);
            }
        }
        updateUnlockedStatus();
    }

    private void updateUnlockedStatus()
    {
        for(int i = 0; i < achievementsTotal.Count; i++)
        {
            if (achievementsTotal[i].unlocked && !achievementsTotal[i].isHidden)
            {
                achievementObjects[i].UnlockAchievement();
            }
            else if (achievementsTotal[i].unlocked && achievementsTotal[i].isHidden)
            {
                achievementObjects[i].setIcon(achievementsTotal[i].icon);
                achievementObjects[i].setTitle(achievementsTotal[i].title);
                achievementObjects[i].setDescription(achievementsTotal[i].description);
                achievementObjects[i].UnlockAchievement();
            }
        }
    }

    public void CheckEmptyRequirement(int achievementID)
    {
        for(int i = 0; i < achievementsTotal.Count; i++)
        {
            if(achievementID == achievementsTotal[i].achievementID && !achievementsTotal[i].unlocked)
            {
                UnlockAchievement(i);
                return;
            }
        }
    }

    public void CheckValueRequirement(int achievementID, int? intValue, float? floatValue)
    {
        for (int i = 0; i < achievementsTotal.Count; i++)
        {
            if (achievementID == achievementsTotal[i].achievementID && !achievementsTotal[i].unlocked)
            {
                if (intValue != null)
                {
                    if (intValue == achievementsTotal[i].intGoalAmount)
                    {
                        UnlockAchievement(i);
                        return;
                    }
                }
                else if (floatValue != null)
                {
                    if (floatValue == achievementsTotal[i].floatGoalAmount)
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
        achievementsTotal[achievementID].unlocked = true;
        updateUnlockedStatus();
        AddToQueue(achievementID);
    }

    private void AddToQueue(int achievementID)
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
        achievementPrefabPopup.UnlockAchievement();
        achievementPrefabPopup.setIcon(achievementsTotal[achievementID].icon);
        achievementPrefabPopup.setTitle(achievementsTotal[achievementID].title);
        achievementPrefabPopup.setDescription(achievementsTotal[achievementID].description);
        achievementPrefabPopup.PlayDisplayAnim();
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
