using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementObject : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image locked;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Animator popupAnimation;
    public void setIcon(Sprite iconInfo)
    {
        icon.sprite = iconInfo;
    }

    public void setTitle(string titleinfo)
    {
        titleText.text = titleinfo;
    }

    public void setDescription(string descriptionInfo)
    {
        descriptionText.text = descriptionInfo;
    }

    public void lockRegularAchievement()
    {
        locked.enabled = true;
        icon.color = new Color(125, 125, 125);
    }

    public void UnlockAchievement()
    {
        locked.enabled = false;
        icon.color = new Color(255, 255, 255);
    }

    public void RemoveLockIcon()
    {
        locked.enabled = false;
    }

    public void PlayDisplayAnim()
    {
        popupAnimation.Play("Displaying");
    }

    public void PlayHideAnim()
    {
        popupAnimation.Play("Hiding");
    }
}
