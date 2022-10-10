using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AchievementInfo)), CanEditMultipleObjects]
public class AchievementEditor : Editor
{
    private SerializedProperty
        _idValueProp,
        _titleValueProp,
        _desciptionValueProp,
        _iconValueProp,
        _completionTypeProp,
        _intValueProp,
        _floatValueProp,
        _isHiddenValueProp,
        _unlockedValueProp,
        _soundEffectValueProp;
    private MonoScript _monoScript;

    private void OnEnable()
    {
        _idValueProp = serializedObject.FindProperty("_achievementId");
        _titleValueProp = serializedObject.FindProperty("_title");
        _desciptionValueProp = serializedObject.FindProperty("_description");
        _iconValueProp = serializedObject.FindProperty("_icon");
        _completionTypeProp = serializedObject.FindProperty("_valueCompletionType");
        _intValueProp = serializedObject.FindProperty("_intGoalAmount");
        _floatValueProp = serializedObject.FindProperty("_floatGoalAmount");
        _isHiddenValueProp = serializedObject.FindProperty("_isHidden");
        _unlockedValueProp = serializedObject.FindProperty("_unlocked");
        _soundEffectValueProp = serializedObject.FindProperty("_soundEffect");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        if(target as MonoBehaviour != null)
        {
            _monoScript = MonoScript.FromMonoBehaviour((MonoBehaviour)target);
        }
        else
        {
            _monoScript = MonoScript.FromScriptableObject((ScriptableObject)target);
        }
        EditorGUILayout.ObjectField("Script", _monoScript, GetType(), false);
        EditorGUI.EndDisabledGroup();
        serializedObject.Update();
        EditorGUILayout.PropertyField(_idValueProp, new GUILayoutOption[] {GUILayout.Width(275)});
        EditorGUILayout.PropertyField(_titleValueProp, new GUILayoutOption[] { GUILayout.Width(500)});
        EditorGUILayout.PropertyField(_desciptionValueProp, new GUILayoutOption[] {GUILayout.Width(500)});
        EditorGUILayout.PropertyField(_iconValueProp, new GUILayoutOption[] {GUILayout.Width(350)});
        EditorGUILayout.PropertyField(_completionTypeProp, new GUILayoutOption[] {GUILayout.Width(400)});
        AchievementInfo.completionType type = (AchievementInfo.completionType)_completionTypeProp.enumValueIndex;

        switch (type)
        {
            case AchievementInfo.completionType.noRequirements:
                break;
            case AchievementInfo.completionType.integerRequirement:
                EditorGUILayout.PropertyField(_intValueProp, new GUILayoutOption[] {GUILayout.Width(300)});
                break;
            case AchievementInfo.completionType.floatRequirement:
                EditorGUILayout.PropertyField(_floatValueProp, new GUILayoutOption[] { GUILayout.Width(350) });
                break;
        }
        EditorGUILayout.PropertyField(_isHiddenValueProp);
        EditorGUILayout.PropertyField(_unlockedValueProp);
        EditorGUILayout.PropertyField(_soundEffectValueProp);
        serializedObject.ApplyModifiedProperties();
    }
}
