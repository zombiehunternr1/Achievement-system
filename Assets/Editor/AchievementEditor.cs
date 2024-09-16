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
        _collectableProp,
        _prevousAchievementProp,
        _collectableListProp,
        _collectableRequirementTypeProp,
        _intCurrentValueProp,
        _intGoalValueProp,
        _floatCurrentValueProp,
        _floatGoalValueProp,
        _achievementListProp,
        _manualGoalAmountProp,
        _requiresPreviousAchievementProp,
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
        _completionTypeProp = serializedObject.FindProperty("_completionType");
        _collectableTypeProp = serializedObject.FindProperty("_collectableType");
        _collectableRequirementTypeProp = serializedObject.FindProperty("_collectableRequirementType");
        _collectableProp = serializedObject.FindProperty("_collectable");
        _prevousAchievementProp = serializedObject.FindProperty("_previousAchievement");
        _collectableListProp = serializedObject.FindProperty("_collectableList");
        _intCurrentValueProp = serializedObject.FindProperty("_intCurrentAmount");
        _intGoalValueProp = serializedObject.FindProperty("_intGoalAmount");
        _floatCurrentValueProp = serializedObject.FindProperty("_floatCurrentAmount");
        _floatGoalValueProp = serializedObject.FindProperty("_floatGoalAmount");
        _achievementListProp = serializedObject.FindProperty("_achievementList");
        _showProgresssionProp = serializedObject.FindProperty("_showProgression");
        _manualGoalAmountProp = serializedObject.FindProperty("_manualGoalAmount");
        _requiresPreviousAchievementProp = serializedObject.FindProperty("_requiresPreviousAchievement");
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
        EditorGUILayout.PropertyField(_titleValueProp);
        EditorGUILayout.PropertyField(_desciptionValueProp);
        EditorGUILayout.PropertyField(_iconValueProp);
        EditorGUILayout.PropertyField(_completionTypeProp, new GUILayoutOption[] {GUILayout.Width(400)});
        AchievementInfoSO.CompletionEnumType completionType = (AchievementInfoSO.CompletionEnumType)_completionTypeProp.enumValueIndex;
        AchievementInfoSO.CollectableEnumType collectableType = (AchievementInfoSO.CollectableEnumType)_collectableTypeProp.enumValueIndex;
        AchievementInfoSO.CollectableRequirementEnumType collectableRequirementType = (AchievementInfoSO.CollectableRequirementEnumType)_collectableRequirementTypeProp.enumValueIndex;
        switch (completionType)
        {
            case AchievementInfoSO.CompletionEnumType.NoRequirements:
                break;
            case AchievementInfoSO.CompletionEnumType.IntegerRequirement:
                EditorGUILayout.PropertyField(_collectableTypeProp, new GUILayoutOption[] { GUILayout.Width(400) });
                switch (collectableType)
                {
                    case AchievementInfoSO.CollectableEnumType.None:
                        EditorGUILayout.PropertyField(_intCurrentValueProp, new GUILayoutOption[] { GUILayout.Width(300) });
                        EditorGUILayout.PropertyField(_intGoalValueProp, new GUILayoutOption[] { GUILayout.Width(300) });
                        break;
                    case AchievementInfoSO.CollectableEnumType.Collectable:
                        EditorGUILayout.PropertyField(_collectableRequirementTypeProp, new GUILayoutOption[] { GUILayout.Width(400) });
                        switch (collectableRequirementType)
                        {
                            case AchievementInfoSO.CollectableRequirementEnumType.Single:
                                EditorGUILayout.PropertyField(_collectableProp, new GUILayoutOption[] { GUILayout.Width(400) });
                                EditorGUILayout.PropertyField(_requiresPreviousAchievementProp);
                                if (_requiresPreviousAchievementProp.boolValue)
                                {
                                    EditorGUILayout.PropertyField(_prevousAchievementProp, new GUILayoutOption[] { GUILayout.Width(400) });
                                }
                                break;
                            case AchievementInfoSO.CollectableRequirementEnumType.List:
                                EditorGUILayout.PropertyField(_collectableListProp, new GUILayoutOption[] { GUILayout.Width(400) });
                                EditorGUILayout.PropertyField(_requiresPreviousAchievementProp);
                                if (_requiresPreviousAchievementProp.boolValue)
                                {
                                    EditorGUILayout.PropertyField(_prevousAchievementProp, new GUILayoutOption[] { GUILayout.Width(400) });
                                }
                                if (_manualGoalAmountProp.boolValue)
                                {
                                    EditorGUILayout.PropertyField(_intGoalValueProp, new GUILayoutOption[] { GUILayout.Width(300) });
                                }
                                EditorGUILayout.PropertyField(_manualGoalAmountProp);
                                break;
                        }
                        break;
                    case AchievementInfoSO.CollectableEnumType.Achievement:
                        EditorGUILayout.PropertyField(_achievementListProp, new GUILayoutOption[] { GUILayout.Width(400) });
                        EditorGUILayout.PropertyField(_requiresPreviousAchievementProp);
                        if (_requiresPreviousAchievementProp.boolValue)
                        {
                            EditorGUILayout.PropertyField(_prevousAchievementProp, new GUILayoutOption[] { GUILayout.Width(400) });
                        }
                        break;
                }
                break;
            case AchievementInfoSO.CompletionEnumType.FloatRequirement:
                EditorGUILayout.PropertyField(_floatCurrentValueProp, new GUILayoutOption[] { GUILayout.Width(350) });
                EditorGUILayout.PropertyField(_floatGoalValueProp, new GUILayoutOption[] { GUILayout.Width(350) });
                break;
        }
        EditorGUILayout.PropertyField(_showProgresssionProp);
        EditorGUILayout.PropertyField(_isHiddenValueProp);
        EditorGUILayout.PropertyField(_unlockedValueProp);
        EditorGUILayout.PropertyField(_soundEffectValueProp);
        serializedObject.ApplyModifiedProperties();
    }
}
