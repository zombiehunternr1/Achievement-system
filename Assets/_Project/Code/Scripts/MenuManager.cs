using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] AchievementEventValue floatAchievement;
    [SerializeField] AchievementEventValue intAchievement;
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
}
