using UnityEngine;
using UnityEngine.UI;

public class ButtonUIController : MonoBehaviour
{
    [SerializeField] private CollectableItem _overTimeReference;
    [Header("Component references")]
    [SerializeField] private Image _holdButton1ImageReference;
    [SerializeField] private Image _holdButton2ImageReference;
    [Header("Achievement references")]
    [SerializeField] private AchievementSO _quitAchievementReference;
    [SerializeField] private AchievementSO _hiddenAchievementReference;
    [Header("Event references")]
    [SerializeField] private EventPackage _updateAchievements;
    private void OnEnable()
    {
        UpdateCollectableStatusDisplay();
    }
    public void UpdateCollectableStatusDisplay()
    {
        _holdButton1ImageReference.fillAmount = (_overTimeReference.CurrentAmountFromList(0) / _overTimeReference.GoalAmountFromList(0));
        _holdButton2ImageReference.fillAmount = (_overTimeReference.CurrentAmountFromList(1) / _overTimeReference.GoalAmountFromList(1));
    }
    public void QuitButton()
    {
        EventPackageFactory.BuildAndInvoke(_updateAchievements, _quitAchievementReference.AchievementId);
    }
    public void HiddenButton()
    {
        EventPackageFactory.BuildAndInvoke(_updateAchievements, _hiddenAchievementReference.AchievementId);
    }
    public void HoldButton1(int index)
    {
        float currentAmount = _overTimeReference.CurrentAmountFromList(index);
        float goalAmount = _overTimeReference.GoalAmountFromList(index);
        _holdButton1ImageReference.fillAmount = currentAmount / goalAmount;
    }
    public void HoldButton2(int index)
    {
        float currentAmount = _overTimeReference.CurrentAmountFromList(index);
        float goalAmount = _overTimeReference.GoalAmountFromList(index);
        _holdButton2ImageReference.fillAmount = currentAmount / goalAmount;
    }
}
