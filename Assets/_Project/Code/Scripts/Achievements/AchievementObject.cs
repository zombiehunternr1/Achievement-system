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
    [SerializeField] private Animator _popupAnimation;
    public void SetIcon(Sprite iconInfo)
    {
        _icon.sprite = iconInfo;
    }
    public void SetTitle(string titleinfo)
    {
        _titleText.text = titleinfo;
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
    public void ProgressDisplay(bool display)
    {
        if (display)
        {
            _progressBarRect.gameObject.SetActive(true);
        }
        else
        {
            _progressBarRect.gameObject.SetActive(false);
        }
    }
    public void PlayDisplayAnim()
    {
        _popupAnimation.Play("Displaying");
    }
    public void PlayHideAnim()
    {
        _popupAnimation.Play("Hiding");
    }
}
