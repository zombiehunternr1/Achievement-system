using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private AchievementReferenceHolderSO _quitAchievementReference;
    [SerializeField] private AchievementReferenceHolderSO _hiddenAchievementReference;
    [SerializeField] private AchievementReferenceHolderSO _floatAchievementReference;
    [SerializeField] private AchievementReferenceHolderSO _intAchievementReference;
    [SerializeField] private UpdateAchievementsEvent _updateAchievmentsEvent;
    [SerializeField] private TMP_InputField _floatInputText;
    [SerializeField] private TMP_InputField _integerInputText;
    public void SubmitFloatValue()
    {
        float floatValue = float.Parse(_floatInputText.text);
        _updateAchievmentsEvent.Invoke(_floatAchievementReference.achievementId, null, floatValue);
    }
    public void SubmitIntValue()
    {
        int intValue = int.Parse(_integerInputText.text);
        _updateAchievmentsEvent.Invoke(_intAchievementReference.achievementId, intValue, null);
    }
    public void QuitButton()
    {
        _updateAchievmentsEvent.Invoke(_quitAchievementReference.achievementId, null, null);
    }
    public void HiddenButton()
    {
        _updateAchievmentsEvent.Invoke(_hiddenAchievementReference.achievementId, null, null);
    }
}
