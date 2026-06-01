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
    private Coroutine _popupCooldownCoroutine;
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

            UpdateAchievementObject(i, achievement, false, string.Empty, achievement.IsHidden);
        }
    }

    public void UpdateAchievementStatus(EventPayload payload)
    {
        AchievementStatusPayload status = EventReader.Get<AchievementStatusPayload>(payload);
        AchievementType achievement = status.Achievement;
        int objectIndex = _achievementObjects.FindIndex(obj => obj.AchievementId == achievement.AchievementId);

        if (objectIndex == -1)
        {
            Debug.LogWarning($"[AchievementUI] No achievement object found matching: {achievement.Title}");
            return;
        }

        bool shouldDisplayAsHidden = !status.IsUnlocked && achievement.IsHidden;
        UpdateAchievementObject(objectIndex, achievement, status.IsUnlocked, status.ProgressionDisplay, shouldDisplayAsHidden);
    }

    public void AchievementUnlocked(EventPayload payload)
    {
        AchievementStatusPayload status = EventReader.Get<AchievementStatusPayload>(payload);

        if (_queuedAchievements.Exists(a => a.AchievementId == status.Achievement.AchievementId))
        {
            return;
        }

        AddToQueueDisplay(status.Achievement);
    }

    public void StartPopupCooldown()
    {
        if (_popupCooldownCoroutine != null)
        {
            StopCoroutine(_popupCooldownCoroutine);
        }

        _popupCooldownCoroutine = StartCoroutine(PopupCooldown());
    }

    public void ClearAchievementQueue()
    {
        if (_popupCooldownCoroutine != null)
        {
            StopCoroutine(_popupCooldownCoroutine);
            _popupCooldownCoroutine = null;
        }

        _queuedAchievements.Clear();
    }

    private void UpdateAchievementObject(int objectIndex, AchievementType achievement,
        bool isUnlocked, string progressionDisplay, bool isHidden)
    {
        AchievementObject achievementObject = _achievementObjects[objectIndex];

        if (isUnlocked)
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
            progressionDisplay,
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
            _popupCooldownCoroutine = null;
        }
    }
}