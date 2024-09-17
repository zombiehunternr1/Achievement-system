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
    public void ProgressDisplay(bool display, int? currentIntValue, int? goalIntValue, float? currentfloatValue, float? goalFloatValue)
    {
        if (display)
        {
            _progressBarRect.gameObject.SetActive(true);
            if(goalIntValue != 0)
            {
                _progressText.text = currentIntValue + " / " + goalIntValue;
                _progressSlider.maxValue = (float)goalIntValue;
                _progressSlider.value = (float)currentIntValue;
            }
            if(goalFloatValue != 0)
            {
                _progressText.text = currentfloatValue + " / " + goalFloatValue;
                _progressSlider.maxValue = (float)goalFloatValue;
                _progressSlider.value = (float)currentfloatValue;
            }
        }
        else
        {
            _progressBarRect.gameObject.SetActive(false);
        }
    }
}
