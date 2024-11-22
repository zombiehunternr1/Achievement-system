using UnityEngine;

public class ButtonUIController : MonoBehaviour
{
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
}
