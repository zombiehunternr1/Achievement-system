using UnityEngine;
using UnityEngine.UI;

public class ButtonUIController : MonoBehaviour
{
    [Header("Component references")]
    [SerializeField] private Image _holdButton1ImageReference;
    [SerializeField] private Image _holdButton2ImageReference;
    [Header("Achievement references")]
    [SerializeField] private AchievementSO _quitAchievementReference;
    [SerializeField] private AchievementSO _hiddenAchievementReference;
    [Header("Event references")]
    [SerializeField] private DoubleEvent _updateAchievementsEvent;
    public void QuitButton()
    {
        _updateAchievementsEvent.Invoke(_quitAchievementReference.AchievementId, null);
    }
    public void HiddenButton()
    {
        _updateAchievementsEvent.Invoke(_hiddenAchievementReference.AchievementId, null);
    }
    public void HoldButton1()
    {

    }
    public void HoldButton2()
    {

    }
}
