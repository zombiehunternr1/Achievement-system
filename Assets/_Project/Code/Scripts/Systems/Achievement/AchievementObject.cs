using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementObject : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Image _locked;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private RectTransform _progressBarRect;
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private Slider _progressSlider;
    private string _achievementId;

    public string AchievementId
    {
        get
        {
            return _achievementId;
        }
    }
    public void SetAchievementId(string achievementId)
    {
        _achievementId = achievementId;
    }
    public void SetIcon(Sprite iconInfo)
    {
        _icon.sprite = iconInfo;
    }
    public void SetTitle(string titleInfo)
    {
        _titleText.text = titleInfo;
    }
    public void SetDescription(string descriptionInfo)
    {
        _descriptionText.text = descriptionInfo;
    }
    public void UnlockAchievement()
    {
        _locked.enabled = false;
        _icon.color = new Color32(255, 255, 255, 255);
    }
    public void EnableLock()
    {
        _locked.enabled = true;
        _icon.color = new Color32(125,125,125, 255);
    }
    public void DisableLock()
    {
        _locked.enabled = false;
    }
    public void ProgressDisplay(bool display, string progression)
    {
        if (!display)
        {
            _progressBarRect.gameObject.SetActive(false);
            _progressSlider.gameObject.SetActive(false);
            return;
        }
        _progressBarRect.gameObject.SetActive(true);
        _progressText.text = progression;
        if (progression.Contains("/"))
        {
            string[] parts = progression.Split('/');
            if (parts.Length == 2)
            {
                if (int.TryParse(parts[0].Trim(), out int currentAmount) &&
                    int.TryParse(parts[1].Trim(), out int goalAmount))
                {
                    _progressSlider.maxValue = goalAmount;
                    _progressSlider.value = currentAmount;
                    _progressSlider.gameObject.SetActive(true);
                }
            }
        }
        else if (progression.Contains("%"))
        {
            string percentageString = progression.Replace("%", "").Trim();
            if (float.TryParse(percentageString, out float percentage))
            {
                _progressSlider.maxValue = 100f;
                _progressSlider.value = percentage;
                _progressSlider.gameObject.SetActive(true);
            }
        }
    }
}
