using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementManager : MonoBehaviour
{
    [SerializeField] private Sprite hiddenAchievement;
    [SerializeField] private RectTransform achievementContainer;
    [SerializeField] private AchievementObject achievementPrefab;
    [SerializeField] private List<AchievementInfo> achievementsTotal;

    private List<AchievementObject> achievementObjects;
    private string hiddenText = "??????????????";

    private void OnEnable()
    {
        achievementObjects = new List<AchievementObject>();
        SetupAchievementDisplay();
    }

    private void SetupAchievementDisplay()
    {
        for (int i = 0; i < achievementsTotal.Count; i++)
        {
            AchievementObject achievementObject = Instantiate(achievementPrefab, achievementContainer);
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
            if(achievementID == achievementsTotal[i].achievementID)
            {
                if (!achievementsTotal[i].unlocked)
                {
                    UnlockAchievement(i);
                    return;
                }
            }
        }
    }

    public void CheckIntRequirement(int achievementID, int value)
    {
        for (int i = 0; i < achievementsTotal.Count; i++)
        {
            if (achievementID == achievementsTotal[i].achievementID)
            {
                if (value == achievementsTotal[i].intGoalAmount)
                {
                    UnlockAchievement(i);
                    return;
                }
            }
        }
    }

    public void CheckFloatRequirement(int achievementID, float value)
    {
        for(int i = 0; i < achievementsTotal.Count; i++)
        {
            if(achievementID == achievementsTotal[i].achievementID)
            {
                if (value == achievementsTotal[i].floatGoalAmount)
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
    }
}
