using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] AchievementEvent floatAchievement;
    [SerializeField] AchievementEvent intAchievement;
    [SerializeField] AchievementEvent quitAchievement;
    [SerializeField] AchievementEvent hiddenAchievement;
    [SerializeField] TMP_InputField floatInputText;
    [SerializeField] TMP_InputField integerInputText;

    public void SubmitFloatValue()
    {
        float floatValue = float.Parse(floatInputText.text);
        floatAchievement.RaiseValueEvent(null, floatValue);
    }

    public void SubmitIntValue()
    {
        int intValue = int.Parse(integerInputText.text);
        intAchievement.RaiseValueEvent(intValue, null);
    }

    public void QuitButton()
    {
        quitAchievement.RaiseValueEvent(null, null);
    }

    public void HiddenButton()
    {
        hiddenAchievement.RaiseValueEvent(null, null);
    }
}
