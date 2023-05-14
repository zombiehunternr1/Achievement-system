using UnityEngine;
using TMPro;

public class MenuController : MonoBehaviour
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
        _updateAchievmentsEvent.Invoke(_floatAchievementReference.AchievementId, null, floatValue);
    }
    public void SubmitIntValue()
    {
        int intValue = int.Parse(_integerInputText.text);
        _updateAchievmentsEvent.Invoke(_intAchievementReference.AchievementId, intValue, null);
    }
    public void QuitButton()
    {
        _updateAchievmentsEvent.Invoke(_quitAchievementReference.AchievementId, null, null);
    }
    public void HiddenButton()
    {
        _updateAchievmentsEvent.Invoke(_hiddenAchievementReference.AchievementId, null, null);
    }
}
