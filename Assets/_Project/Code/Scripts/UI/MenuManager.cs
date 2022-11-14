using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] AchievementEvent _quitAchievementEvent;
    [SerializeField] AchievementEvent _hiddenAchievementEvent;
    [SerializeField] AchievementEvent _floatAchievementEvent;
    [SerializeField] AchievementEvent _intAchievementEvent;
    [SerializeField] TMP_InputField _floatInputText;
    [SerializeField] TMP_InputField _integerInputText;
    public void SubmitFloatValue()
    {
        float floatValue = float.Parse(_floatInputText.text);
        _floatAchievementEvent.Invoke(_floatAchievementEvent.achievementId, null, floatValue);
    }
    public void SubmitIntValue()
    {
        int intValue = int.Parse(_integerInputText.text);
        _intAchievementEvent.Invoke(_intAchievementEvent.achievementId, intValue, null);
    }
    public void QuitButton()
    {
        _quitAchievementEvent.Invoke(_quitAchievementEvent.achievementId, null, null);
    }
    public void HiddenButton()
    {
        _hiddenAchievementEvent.Invoke(_hiddenAchievementEvent.achievementId, null, null);
    }
}
