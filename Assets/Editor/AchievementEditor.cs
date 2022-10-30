using System.Diagnostics;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AchievementInfoSO)), CanEditMultipleObjects]
public class AchievementEditor : Editor
{
    private SerializedProperty
        _idValueProp,
        _titleValueProp,
        _desciptionValueProp,
        _iconValueProp,
        _completionTypeProp,
        _collectableTypeProp,
        _intCurrentValueProp,
        _intGoalValueProp,
        _floatCurrentValueProp,
        _floatGoalValueProp,
        _collectableTypeListProp,
        _showProgresssionProp,
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
        _completionTypeProp = serializedObject.FindProperty("_CompletionType");
        _collectableTypeProp = serializedObject.FindProperty("_CollectableType");
        _intCurrentValueProp = serializedObject.FindProperty("_intCurrentAmount");
        _intGoalValueProp = serializedObject.FindProperty("_intGoalAmount");
        _floatCurrentValueProp = serializedObject.FindProperty("_floatCurrentAmount");
        _floatGoalValueProp = serializedObject.FindProperty("_floatGoalAmount");
        _collectableTypeListProp = serializedObject.FindProperty("_collectable");
        _showProgresssionProp = serializedObject.FindProperty("_showProgression");
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
        EditorGUILayout.PropertyField(_idValueProp, new GUILayoutOption[] {GUILayout.Width(750)});
        EditorGUILayout.PropertyField(_titleValueProp, new GUILayoutOption[] { GUILayout.Width(500)});
        EditorGUILayout.PropertyField(_desciptionValueProp, new GUILayoutOption[] {GUILayout.Width(500)});
        EditorGUILayout.PropertyField(_iconValueProp, new GUILayoutOption[] {GUILayout.Width(350)});
        EditorGUILayout.PropertyField(_completionTypeProp, new GUILayoutOption[] {GUILayout.Width(400)});
        AchievementInfoSO.completionType completionType = (AchievementInfoSO.completionType)_completionTypeProp.enumValueIndex;
        AchievementInfoSO.CollectableType collectableType = (AchievementInfoSO.CollectableType)_collectableTypeProp.enumValueIndex;

        switch (completionType)
        {
            case AchievementInfoSO.completionType.noRequirements:
                break;
            case AchievementInfoSO.completionType.integerRequirement:
                EditorGUILayout.PropertyField(_collectableTypeProp, new GUILayoutOption[] { GUILayout.Width(400) });
                switch (collectableType)
                {
                    case AchievementInfoSO.CollectableType.None:
                        EditorGUILayout.PropertyField(_intCurrentValueProp, new GUILayoutOption[] { GUILayout.Width(300) });
                        EditorGUILayout.PropertyField(_intGoalValueProp, new GUILayoutOption[] { GUILayout.Width(300) });
                        break;
                    case AchievementInfoSO.CollectableType.Collectable:
                        EditorGUILayout.PropertyField(_intGoalValueProp, new GUILayoutOption[] { GUILayout.Width(300) });
                        EditorGUILayout.PropertyField(_collectableTypeListProp, new GUILayoutOption[] { GUILayout.Width(400) });
                        break;
                }
                break;
            case AchievementInfoSO.completionType.floatRequirement:
                EditorGUILayout.PropertyField(_collectableTypeProp, new GUILayoutOption[] { GUILayout.Width(400) });
                switch (collectableType) 
                {
                    case AchievementInfoSO.CollectableType.None:
                        EditorGUILayout.PropertyField(_floatCurrentValueProp, new GUILayoutOption[] { GUILayout.Width(350) });
                        EditorGUILayout.PropertyField(_floatGoalValueProp, new GUILayoutOption[] { GUILayout.Width(350) });
                        break;
                    case AchievementInfoSO.CollectableType.Collectable:
                        EditorGUILayout.PropertyField(_floatGoalValueProp, new GUILayoutOption[] { GUILayout.Width(350) });
                        EditorGUILayout.PropertyField(_collectableTypeListProp, new GUILayoutOption[] { GUILayout.Width(400) });
                        break;
                }
                break;
        }
        EditorGUILayout.PropertyField(_showProgresssionProp);
        EditorGUILayout.PropertyField(_isHiddenValueProp);
        EditorGUILayout.PropertyField(_unlockedValueProp);
        EditorGUILayout.PropertyField(_soundEffectValueProp);
        serializedObject.ApplyModifiedProperties();
    }
}
