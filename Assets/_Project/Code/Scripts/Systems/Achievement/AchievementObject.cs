using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementObject : AchievementDisplayBase
{
    [SerializeField] private Image _locked;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private RectTransform _progressBarRect;
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private Sprite _hiddenAchievementSprite;
    private string _achievementId;
    private const string HiddenText = "??????????????";
    public string AchievementId
    {
        get
        {
            return _achievementId;
        }
    }
    public void SetAchievementData(Sprite icon, string title, string description, bool displayProgression, string Progression, RewardTier rewardTier, bool isHidden)
    {
        if (isHidden)
        {
            SetIconAndTitle(_hiddenAchievementSprite, HiddenText);
            _descriptionText.text = HiddenText;
        }
        else
        {
            SetRewardTier(rewardTier);
            SetIconAndTitle(icon, title);
            _descriptionText.text = description;
        }
        ProgressDisplay(displayProgression, Progression);
    }
    public void SetAchievementId(string achievementId)
    {
        _achievementId = achievementId;
    }
    public void UnlockAchievement()
    {
        DisableLock();
        SetIconColor(new Color32(255, 255, 255, 255));
    }
    public void EnableLock()
    {
        _locked.enabled = true;
        SetIconColor(new Color32(125, 125, 125, 255));
    }
    public void DisableLock()
    {
        _locked.enabled = false;
    }
    private void ProgressDisplay(bool display, string progression)
    {
        _progressBarRect.gameObject.SetActive(display);
        _progressSlider.gameObject.SetActive(display);
        if (!display)
        {
            return;
        }
        _progressText.text = progression;
        if (progression.Contains("/"))
        {
            string[] parts = progression.Split('/');
            if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int currentAmount) && int.TryParse(parts[1].Trim(), out int goalAmount))
            {
                _progressSlider.maxValue = goalAmount;
                _progressSlider.value = currentAmount;
            }
        }
        else if (progression.Contains("%"))
        {
            string percentageString = progression.Replace("%", "").Trim();
            if (float.TryParse(percentageString, out float percentage))
            {
                _progressSlider.maxValue = 100f;
                _progressSlider.value = percentage;
            }
        }
    }
}
