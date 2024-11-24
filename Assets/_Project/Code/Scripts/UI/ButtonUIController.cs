using UnityEngine;
using UnityEngine.UI;

public class ButtonUIController : MonoBehaviour
{
    [SerializeField] private CollectableSO _overTimeReference;
    [Header("Component references")]
    [SerializeField] private Image _holdButton1ImageReference;
    [SerializeField] private Image _holdButton2ImageReference;
    [Header("Achievement references")]
    [SerializeField] private AchievementSO _quitAchievementReference;
    [SerializeField] private AchievementSO _hiddenAchievementReference;
    [Header("Event references")]
    [SerializeField] private DoubleEvent _updateAchievementsEvent;
    private void OnEnable()
    {
        _holdButton1ImageReference.fillAmount = _overTimeReference.CurrentAmountFromList(0) / _overTimeReference.GoalAmountFromList(0);
        _holdButton2ImageReference.fillAmount = _overTimeReference.CurrentAmountFromList(1) / _overTimeReference.GoalAmountFromList(1);
    }
    public void QuitButton()
    {
        _updateAchievementsEvent.Invoke(_quitAchievementReference.AchievementId, null);
    }
    public void HiddenButton()
    {
        _updateAchievementsEvent.Invoke(_hiddenAchievementReference.AchievementId, null);
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
