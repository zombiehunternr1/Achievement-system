using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private EventPackage _playPopUpDisplayStatus;
    [SerializeField] private EventPackage _setAchievementPopUpInfo;
    [SerializeField] private int _displayPopupTime = 5;
    [SerializeField] private RectTransform _achievementContainerRect;
    [SerializeField] private AchievementObject _achievementPrefabContainer;
    private List<AchievementObject> _achievementObjects = new List<AchievementObject>();
    private List<AchievementType> _queuedAchievements = new List<AchievementType>();
    private EventInstance _soundEffect;

    public void StartPopupCooldown()
    {
        StartCoroutine(PopupCooldown());
    }
    public void ClearAchievementQueue()
    {
        _queuedAchievements.Clear();
    }
    public void SetupAchievementDisplay(EventData eventData)
    {
        List<AchievementType> allAchievements = EventPackageExtractor.ExtractEventData<List<AchievementType>>(eventData);
        if (allAchievements.Count == 0)
        {
            Debug.LogWarning("The list of achievements to unlock is empty!");
            return;
        }
        for (int i = 0; i < allAchievements.Count; i++)
        {
            AchievementType achievement = allAchievements[i];
            if (achievement == null)
            {
                Debug.LogWarning("There is a missing reference at element " + i + " in the achievements to unlock list");
                continue;
            }
            AchievementObject achievementObject = Instantiate(_achievementPrefabContainer, _achievementContainerRect);
            achievementObject.SetAchievementId(achievement.AchievementId);
            _achievementObjects.Add(achievementObject);
            if (achievement.IsHidden)
            {
                achievementObject.DisableLock();
            }
            UpdateAchievementObject(i, achievement, achievement.IsHidden);
        }
    }
    public void UpdateAchievementStatus(EventData eventData)
    {
        AchievementType achievement = EventPackageExtractor.ExtractEventData<AchievementType>(eventData);
        int objectIndex = _achievementObjects.FindIndex(obj => obj.AchievementId == achievement.AchievementId);
        if (objectIndex == -1)
        {
            Debug.LogWarning("No corresponding achievement object found with achievement: " + achievement.Title + "!");
            return;
        }
        bool shouldDisplayAsHidden = !achievement.IsUnlocked && achievement.IsHidden;
        UpdateAchievementObject(objectIndex, achievement, shouldDisplayAsHidden);
    }

    public void AchievementUnlocked(EventData eventData)
    {
        AchievementType achievement = EventPackageExtractor.ExtractEventData<AchievementType>(eventData);
        if (_queuedAchievements.Exists(a => a.AchievementId == achievement.AchievementId))
        {
            return;
        }
        AddToQueueDisplay(achievement);
    }
    private void UpdateAchievementObject(int objectIndex, AchievementType achievement, bool isHidden)
    {
        AchievementObject achievementObject = _achievementObjects[objectIndex];
        if (achievement.IsUnlocked)
        {
            achievementObject.UnlockAchievement();
        }
        else if (!achievement.IsHidden)
        {
            achievementObject.EnableLock();
        }
        achievementObject.SetAchievementData(
            achievement.Icon,
            achievement.Title,
            achievement.Description,
            achievement.HasProgressionDisplay,
            achievement.ProgressionDisplay,
            achievement.RewardTier,
            isHidden
        );
    }
    private void AddToQueueDisplay(AchievementType achievement)
    {
        if (_queuedAchievements.Count == 0)
        {
            _queuedAchievements.Add(achievement);
            DisplayPopUpAchievement(achievement);
        }
        else
        {
            _queuedAchievements.Add(achievement);
        }
    }
    private void DisplayNextinQueue()
    {
        _queuedAchievements.RemoveAt(0);
        if (_queuedAchievements.Count != 0)
        {
            DisplayPopUpAchievement(_queuedAchievements[0]);
        }
    }
    private void DisplayPopUpAchievement(AchievementType achievement)
    {
        EventPackageFactory.BuildAndInvoke(_setAchievementPopUpInfo, achievement.Icon, achievement.Title, achievement.RewardTier);
        EventPackageFactory.BuildAndInvoke(_playPopUpDisplayStatus, "Displaying");
        _soundEffect = RuntimeManager.CreateInstance(achievement.SoundEffect);
        RuntimeManager.AttachInstanceToGameObject(_soundEffect, transform);
        _soundEffect.start();
        _soundEffect.release();
    }
    private IEnumerator PopupCooldown()
    {
        yield return new WaitForSeconds(_displayPopupTime);
        EventPackageFactory.BuildAndInvoke(_playPopUpDisplayStatus, "Hiding");
        yield return new WaitForSeconds(1.5f);
        if (_queuedAchievements.Count != 0)
        {
            DisplayNextinQueue();
        }
        else
        {
            StopAllCoroutines();
        }
    }
}
