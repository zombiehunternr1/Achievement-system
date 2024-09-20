using UnityEngine;
using TMPro;

public class MenuController : MonoBehaviour
{
    [SerializeField] private AchievementInfoSO _quitAchievementReference;
    [SerializeField] private AchievementInfoSO _hiddenAchievementReference;
    [SerializeField] private AchievementInfoSO _floatAchievementReference;
    [SerializeField] private AchievementInfoSO _intAchievementReference;
    [SerializeField] private TripleEvent _updateAchievementsEvent;
    [SerializeField] private TMP_InputField _floatInputText;
    [SerializeField] private TMP_InputField _integerInputText;
    public void SubmitFloatValue()
    {
        float floatValue = float.Parse(_floatInputText.text);
        _updateAchievementsEvent.Invoke(_floatAchievementReference.AchievementId, null, floatValue);
    }
    public void SubmitIntValue()
    {
        int intValue = int.Parse(_integerInputText.text);
        _updateAchievementsEvent.Invoke(_intAchievementReference.AchievementId, intValue, null);
    }
    public void QuitButton()
    {
        _updateAchievementsEvent.Invoke(_quitAchievementReference.AchievementId, null, null);
    }
    public void HiddenButton()
    {
        _updateAchievementsEvent.Invoke(_hiddenAchievementReference.AchievementId, null, null);
    }
}
