using UnityEngine;
using TMPro;

public class ValueUIController : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private TMP_InputField _floatInputText;
    [SerializeField] private TMP_InputField _integerInputText;

    [Header("Achievement References")]
    [SerializeField] private AchievementType _floatAchievementReference;
    [SerializeField] private AchievementType _integerAchievementReference;

    [Header("Event Channels")]
    [SerializeField] private EventChannel _updateAchievements;

    public void SubmitFloatValue()
    {
        if (!float.TryParse(_floatInputText.text, out float floatValue))
        {
            Debug.LogWarning("[ValueUIController] Invalid float input: " + _floatInputText.text);
            return;
        }

        EventDispatcher.Raise(_updateAchievements, new AchievementTrigger(_floatAchievementReference.AchievementId, floatValue));
    }

    public void SubmitIntValue()
    {
        if (!int.TryParse(_integerInputText.text, out int intValue))
        {
            Debug.LogWarning("[ValueUIController] Invalid integer input: " + _integerInputText.text);
            return;
        }

        EventDispatcher.Raise(_updateAchievements, new AchievementTrigger(_integerAchievementReference.AchievementId, intValue));
    }
}