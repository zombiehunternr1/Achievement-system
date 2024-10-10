using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AchievementSO)), CanEditMultipleObjects]
public class AchievementSOEditor : Editor
{
    private SerializedProperty
        _achievementIdProp,
        _titleProp,
        _descriptionProp,
        _iconProp,
        _unlockedProp,
        _soundEffectProp,
        _requiresPreviousAchievementProp,
        _previousAchievementProp,
        _achievementListProp,
        _customAchievementGoalAmountProp,
        _goalAchievementAmountProp,
        _isHiddenProp,
        _showProgressionProp,
        _progressionEnumDisplayProp,
        _completionEnumRequirementProp,
        _valueEnumTypeProp,
        _isExactAmountProp,
        _currentIntegerAmountProp,
        _goalIntegerAmountProp,
        _currentFloatAmountProp,
        _goalFloatAmountProp,
        _collectableEnumRequirementProp,
        _collectableTypeProp,
        _collectableTypeListProp,
        _collectableTypeListsProp,
        _requiresMultipleCollectableListsProp,
        _minimumGoalAmountProp;
    private MonoScript _monoScript;
    private void OnEnable()
    {
        _achievementIdProp = serializedObject.FindProperty("_achievementId");
        _titleProp = serializedObject.FindProperty("_title");
        _descriptionProp = serializedObject.FindProperty("_description");
        _iconProp = serializedObject.FindProperty("_icon");
        _unlockedProp = serializedObject.FindProperty("_unlocked");
        _soundEffectProp = serializedObject.FindProperty("_soundEffect");
        _requiresPreviousAchievementProp = serializedObject.FindProperty("_requiresPreviousAchievement");
        _previousAchievementProp = serializedObject.FindProperty("_previousAchievement");
        _achievementListProp = serializedObject.FindProperty("_achievementList");
        _customAchievementGoalAmountProp = serializedObject.FindProperty("_customAchievementGoalAmount");
        _goalAchievementAmountProp = serializedObject.FindProperty("_goalAchievementAmount");
        _isHiddenProp = serializedObject.FindProperty("_isHidden");
        _showProgressionProp = serializedObject.FindProperty("_showProgression");
        _progressionEnumDisplayProp = serializedObject.FindProperty("_progressionEnumDisplay");
        _completionEnumRequirementProp = serializedObject.FindProperty("_completionEnumRequirement");
        _valueEnumTypeProp = serializedObject.FindProperty("_valueEnumType");
        _isExactAmountProp = serializedObject.FindProperty("_isExactAmount");
        _currentIntegerAmountProp = serializedObject.FindProperty("_currentIntegerAmount");
        _goalIntegerAmountProp = serializedObject.FindProperty("_goalIntegerAmount");
        _currentFloatAmountProp = serializedObject.FindProperty("_currentFloatAmount");
        _goalFloatAmountProp = serializedObject.FindProperty("_goalFloatAmount");
        _collectableEnumRequirementProp = serializedObject.FindProperty("_collectableEnumRequirement");
        _collectableTypeProp = serializedObject.FindProperty("_collectableType");
        _collectableTypeListProp = serializedObject.FindProperty("_collectableTypeList");
        _collectableTypeListsProp = serializedObject.FindProperty("_collectableTypeLists");
        _requiresMultipleCollectableListsProp = serializedObject.FindProperty("_requiresMultipleCollectableLists");
        _minimumGoalAmountProp = serializedObject.FindProperty("_minimumGoalAmount");
    }
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        if (target as MonoBehaviour != null)
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
        EditorGUILayout.PropertyField(_achievementIdProp);
        EditorGUILayout.PropertyField(_titleProp);
        EditorGUILayout.PropertyField(_descriptionProp);
        EditorGUILayout.PropertyField(_iconProp);
        EditorGUILayout.PropertyField(_soundEffectProp);
        EditorGUILayout.PropertyField(_unlockedProp);
        EditorGUILayout.PropertyField(_requiresPreviousAchievementProp);
        if (_requiresPreviousAchievementProp.boolValue)
        {
            EditorGUILayout.PropertyField(_previousAchievementProp);
        }
        else
        {
            _previousAchievementProp.objectReferenceValue = null;
        }
        EditorGUILayout.PropertyField(_isHiddenProp);
        if (!_isHiddenProp.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_showProgressionProp);
            if (_showProgressionProp.boolValue)
            {
                EditorGUILayout.PropertyField(_progressionEnumDisplayProp);
            }
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Completion settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_completionEnumRequirementProp);
        CompletionEnumRequirement completionEnumType = (CompletionEnumRequirement)_completionEnumRequirementProp.enumValueIndex;
        ValueEnumType valueEnumType = (ValueEnumType)_valueEnumTypeProp.enumValueIndex;
        CollectableEnumRequirement collectableEnumRequirement = (CollectableEnumRequirement)_collectableEnumRequirementProp.enumValueIndex;
        switch (completionEnumType)
        {
            case CompletionEnumRequirement.NoRequirement:
            break;
            case CompletionEnumRequirement.ValueRequirement:
                EditorGUILayout.PropertyField(_valueEnumTypeProp);
                EditorGUILayout.PropertyField(_isExactAmountProp);
                switch (valueEnumType)
                {
                    case ValueEnumType.Integer:
                        EditorGUILayout.PropertyField(_currentIntegerAmountProp);
                        EditorGUILayout.PropertyField(_goalIntegerAmountProp);
                    break;
                    case ValueEnumType.Float:
                        EditorGUILayout.PropertyField(_currentFloatAmountProp);
                        EditorGUILayout.PropertyField (_goalFloatAmountProp);
                    break;
                }
            break;
            case CompletionEnumRequirement.CollectableRequirement:
                EditorGUILayout.PropertyField(_collectableEnumRequirementProp);
                switch (collectableEnumRequirement)
                {
                    case CollectableEnumRequirement.SingleCollectable:
                        EditorGUILayout.PropertyField(_collectableTypeProp);
                    break;
                    case CollectableEnumRequirement.AllCollectables:
                        EditorGUILayout.PropertyField(_collectableTypeListProp);
                    break;
                    case CollectableEnumRequirement.Custom:
                        EditorGUILayout.PropertyField(_requiresMultipleCollectableListsProp);
                        if (_requiresMultipleCollectableListsProp.boolValue)
                        {
                            EditorGUILayout.PropertyField(_collectableTypeListsProp);
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(_collectableTypeListProp);
                        }
                        EditorGUILayout.PropertyField(_minimumGoalAmountProp);
                    break;
                }
            break;
            case CompletionEnumRequirement.AchievementRequirement:
                EditorGUILayout.PropertyField(_achievementListProp);
                EditorGUILayout.PropertyField(_customAchievementGoalAmountProp);
                if (_customAchievementGoalAmountProp.boolValue)
                {
                    EditorGUILayout.PropertyField(_goalAchievementAmountProp);
                }
            break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
