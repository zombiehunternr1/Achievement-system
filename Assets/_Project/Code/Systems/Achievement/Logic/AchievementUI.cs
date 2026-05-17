using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementUI : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private EventChannel _setAchievementPopUpInfo;
    [SerializeField] private EventChannel _playPopUpDisplayStatus;

    [Header("Display Settings")]
    [SerializeField] private int _displayPopupTime = 5;

    [Header("UI References")]
    [SerializeField] private RectTransform _achievementContainerRect;
    [SerializeField] private AchievementObject _achievementPrefabContainer;

    private readonly List<AchievementObject> _achievementObjects = new List<AchievementObject>();
    private readonly List<AchievementType> _queuedAchievements = new List<AchievementType>();
    private EventInstance _soundEffect;

    public void SetupAchievementDisplay(EventPayload payload)
    {
        List<AchievementType> allAchievements = EventReader.Get<List<AchievementType>>(payload);

        if (allAchievements == null || allAchievements.Count == 0)
        {
            Debug.LogWarning("[AchievementUI] Achievement list is empty or missing.");
            return;
        }

        for (int i = 0; i < allAchievements.Count; i++)
        {
            AchievementType achievement = allAchievements[i];

            if (achievement == null)
            {
                Debug.LogWarning($"[AchievementUI] Missing reference at element {i} in the achievement list.");
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

    public void UpdateAchievementStatus(EventPayload payload)
    {
        AchievementType achievement = EventReader.Get<AchievementType>(payload);
        int objectIndex = _achievementObjects.FindIndex(obj => obj.AchievementId == achievement.AchievementId);

        if (objectIndex == -1)
        {
            Debug.LogWarning($"[AchievementUI] No achievement object found matching: {achievement.Title}");
            return;
        }

        bool shouldDisplayAsHidden = !achievement.IsUnlocked && achievement.IsHidden;
        UpdateAchievementObject(objectIndex, achievement, shouldDisplayAsHidden);
    }

    public void AchievementUnlocked(EventPayload payload)
    {
        AchievementType achievement = EventReader.Get<AchievementType>(payload);

        if (_queuedAchievements.Exists(a => a.AchievementId == achievement.AchievementId))
        {
            return;
        }

        AddToQueueDisplay(achievement);
    }

    public void StartPopupCooldown()
    {
        StartCoroutine(PopupCooldown());
    }

    public void ClearAchievementQueue()
    {
        _queuedAchievements.Clear();
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
        _queuedAchievements.Add(achievement);

        if (_queuedAchievements.Count == 1)
        {
            DisplayPopUpAchievement(achievement);
        }
    }

    private void DisplayNextInQueue()
    {
        _queuedAchievements.RemoveAt(0);

        if (_queuedAchievements.Count > 0)
        {
            DisplayPopUpAchievement(_queuedAchievements[0]);
        }
    }

    private void DisplayPopUpAchievement(AchievementType achievement)
    {
        EventDispatcher.Raise(_setAchievementPopUpInfo, achievement.Icon, achievement.Title, achievement.RewardTier);
        EventDispatcher.Raise(_playPopUpDisplayStatus, "Displaying");

        _soundEffect = RuntimeManager.CreateInstance(achievement.SoundEffect);
        RuntimeManager.AttachInstanceToGameObject(_soundEffect, transform);
        _soundEffect.start();
        _soundEffect.release();
    }

    private IEnumerator PopupCooldown()
    {
        yield return new WaitForSeconds(_displayPopupTime);
        EventDispatcher.Raise(_playPopUpDisplayStatus, "Hiding");
        yield return new WaitForSeconds(1.5f);

        if (_queuedAchievements.Count > 0)
        {
            DisplayNextInQueue();
        }
        else
        {
            StopAllCoroutines();
        }
    }
}