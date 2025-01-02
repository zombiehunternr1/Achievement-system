using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class AchievementDisplayBase : MonoBehaviour
{
    [SerializeField] protected Image _tierBackground;
    [SerializeField] protected Image _icon;
    [SerializeField] protected TextMeshProUGUI _title;
    public void SetRewardTier(RewardTier rewardTier)
    {
        switch (rewardTier)
        {
            case RewardTier.Bronze:
                _tierBackground.color = new Color32(205, 127, 50, 255);
                break;
            case RewardTier.Silver:
                _tierBackground.color = new Color32(192, 192, 192, 255);
                break;
            case RewardTier.Gold:
                _tierBackground.color = new Color32(255, 215, 0, 255);
                break;
            case RewardTier.Platinum:
                _tierBackground.color = new Color32(220, 220, 255, 255);
                break;
        }
    }
    public void SetIconAndTitle(Sprite icon, string title)
    {
        _icon.sprite = icon;
        _title.text = title;
    }
    public void SetIconColor(Color color)
    {
        _icon.color = color;
    }
}
