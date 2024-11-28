using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementPopUpDisplay : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Animator _popupAnimation;
    public void SetPopUpInfo(object iconInfoObj, object titleInfoObj)
    {
        Sprite iconInfo = (Sprite)iconInfoObj;
        string titleInfo = (string)titleInfoObj;
        _icon.sprite = iconInfo;
        _titleText.text = titleInfo;
    }
    public void PlayDisplayStatus(object displayStatusObj)
    {
        string displayStatus = (string)displayStatusObj;
        _popupAnimation.Play(displayStatus);
    }
}
