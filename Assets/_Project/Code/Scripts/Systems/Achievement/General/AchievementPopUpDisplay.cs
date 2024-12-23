using UnityEngine;

public class AchievementPopUpDisplay : AchievementDisplayBase
{
    [SerializeField] private Animator _popupAnimatorReference;
    public void SetPopUpInfo(EventData eventData)
    {
        Sprite iconInfo = EventPackageExtractor.ExtractEventData<Sprite>(eventData);
        string titleInfo = EventPackageExtractor.ExtractEventData<string>(eventData);
        RewardTier rewardTier = EventPackageExtractor.ExtractEventData<RewardTier>(eventData);
        SetIconAndTitle(iconInfo, titleInfo);
        SetRewardTier(rewardTier);
    }
    public void PlayDisplayStatus(EventData eventData)
    {
        string displayStatus = EventPackageExtractor.ExtractEventData<string>(eventData);
        _popupAnimatorReference.Play(displayStatus);
    }
}
