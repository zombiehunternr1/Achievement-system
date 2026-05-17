using UnityEngine;

public class AchievementPopUpDisplay : AchievementDisplayBase
{
    [SerializeField] private Animator _popupAnimatorReference;

    public void SetPopUpInfo(EventPayload payload)
    {
        Sprite iconInfo = EventReader.Get<Sprite>(payload);
        string titleInfo = EventReader.Get<string>(payload);
        RewardTier rewardTier = EventReader.Get<RewardTier>(payload);

        SetIconAndTitle(iconInfo, titleInfo);
        SetRewardTier(rewardTier);
    }

    public void PlayDisplayStatus(EventPayload payload)
    {
        string displayStatus = EventReader.Get<string>(payload);
        _popupAnimatorReference.Play(displayStatus);
    }
}