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
        checkUnlockStatus();
    }

    private void checkUnlockStatus()
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
}
