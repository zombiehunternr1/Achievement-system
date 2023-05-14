using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementPopUpDisplay : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Animator _popupAnimation;
    public void SetPopUpInfo(Sprite iconInfo, string titleInfo)
    {
        _icon.sprite = iconInfo;
        _titleText.text = titleInfo;
    }
    public void PlayDisplayStatus(string displayStatus)
    {
        _popupAnimation.Play(displayStatus);
    }
}
