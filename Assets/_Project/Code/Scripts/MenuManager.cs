using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] AchievementEvent _floatAchievement;
    [SerializeField] AchievementEvent _intAchievement;
    [SerializeField] AchievementEvent _quitAchievement;
    [SerializeField] AchievementEvent _hiddenAchievement;
    [SerializeField] TMP_InputField _floatInputText;
    [SerializeField] TMP_InputField _integerInputText;
    public void SubmitFloatValue()
    {
        float floatValue = float.Parse(_floatInputText.text);
        _floatAchievement.RaiseValueEvent(null, floatValue);
    }
    public void SubmitIntValue()
    {
        int intValue = int.Parse(_integerInputText.text);
        _intAchievement.RaiseValueEvent(intValue, null);
    }
    public void QuitButton()
    {
        _quitAchievement.RaiseValueEvent(null, null);
    }
    public void HiddenButton()
    {
        _hiddenAchievement.RaiseValueEvent(null, null);
    }
}
