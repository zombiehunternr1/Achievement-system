using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementObject : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Image _locked;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Animator _popupAnimation;
    public void setIcon(Sprite iconInfo)
    {
        _icon.sprite = iconInfo;
    }

    public void setTitle(string titleinfo)
    {
        _titleText.text = titleinfo;
    }

    public void setDescription(string descriptionInfo)
    {
        _descriptionText.text = descriptionInfo;
    }

    public void lockRegularAchievement()
    {
        _locked.enabled = true;
        _icon.color = new Color(125, 125, 125);
    }

    public void UnlockAchievement()
    {
        _locked.enabled = false;
        _icon.color = new Color(255, 255, 255);
    }

    public void RemoveLockIcon()
    {
        _locked.enabled = false;
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
