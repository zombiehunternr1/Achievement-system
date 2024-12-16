using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementPopUpDisplay : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Animator _popupAnimatorReference;
    public void SetPopUpInfo(EventData eventData)
    {
        Sprite iconInfo = EventPackageExtractor.ExtractEventData<Sprite>(eventData);
        string titleInfo = EventPackageExtractor.ExtractEventData<string>(eventData);
        _icon.sprite = iconInfo;
        _titleText.text = titleInfo;
    }
    public void PlayDisplayStatus(EventData eventData)
    {
        string displayStatus = EventPackageExtractor.ExtractEventData<string>(eventData);
        _popupAnimatorReference.Play(displayStatus);
    }
}
