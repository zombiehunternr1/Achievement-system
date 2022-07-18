using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AchievementInfo)), CanEditMultipleObjects]
public class AchievementEditor : Editor
{
    public SerializedProperty
        titleValueProp,
        desciptionValueProp,
        iconValueProp,
        completionTypeProp,
        intValueProp,
        floatValueProp,
        isHiddenValueProp,
        unlockedValueProp;

    private void OnEnable()
    {
        titleValueProp = serializedObject.FindProperty("title");
        desciptionValueProp = serializedObject.FindProperty("description");
        iconValueProp = serializedObject.FindProperty("icon");
        completionTypeProp = serializedObject.FindProperty("valueCompletionType");
        intValueProp = serializedObject.FindProperty("intGoalAmount");
        floatValueProp = serializedObject.FindProperty("floatGoalAmount");
        isHiddenValueProp = serializedObject.FindProperty("isHidden");
        unlockedValueProp = serializedObject.FindProperty("unlocked");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(titleValueProp, new GUILayoutOption[] { GUILayout.Width(500)});
        EditorGUILayout.PropertyField(desciptionValueProp, new GUILayoutOption[] {GUILayout.Width(500)});
        EditorGUILayout.PropertyField(iconValueProp, new GUILayoutOption[] {GUILayout.Width(350)});
        EditorGUILayout.PropertyField(completionTypeProp, new GUILayoutOption[] { GUILayout.Width(400)});
        AchievementInfo.completionType type = (AchievementInfo.completionType)completionTypeProp.enumValueIndex;

        switch (type)
        {
            case AchievementInfo.completionType.noRequirements:
                break;
            case AchievementInfo.completionType.integerRequirement:
                EditorGUILayout.PropertyField(intValueProp, new GUILayoutOption[] {GUILayout.Width(300)});
                break;
            case AchievementInfo.completionType.floatRequirement:
                EditorGUILayout.PropertyField(floatValueProp, new GUILayoutOption[] { GUILayout.Width(350) });
                break;
        }
        EditorGUILayout.PropertyField(isHiddenValueProp);
        EditorGUILayout.PropertyField(unlockedValueProp);
        serializedObject.ApplyModifiedProperties();
    }
}
