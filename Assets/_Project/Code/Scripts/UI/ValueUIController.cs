using UnityEngine;
using TMPro;

public class ValueUIController : MonoBehaviour
{
    [Header("Component references")]
    [SerializeField] private TMP_InputField _floatInputText;
    [SerializeField] private TMP_InputField _integerInputText;
    [Header("Achievement references")]
    [SerializeField] private AchievementSO _floatAchievementReference;
    [SerializeField] private AchievementSO _integerAchievementReference;
    [Header("Event references")]
    [SerializeField] private DoubleEvent _updateAchievementsEvent;
    public void SubmitFloatValue()
    {
        float floatValue = float.Parse(_floatInputText.text);
        _updateAchievementsEvent.Invoke(_floatAchievementReference.AchievementId, floatValue);
    }
    public void SubmitIntValue()
    {
        int intValue = int.Parse(_integerInputText.text);
        _updateAchievementsEvent.Invoke(_integerAchievementReference.AchievementId, intValue);
    }
}