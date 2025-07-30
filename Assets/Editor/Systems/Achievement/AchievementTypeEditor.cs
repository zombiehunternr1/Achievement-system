using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(AchievementType)), CanEditMultipleObjects]
public class AchievementTypeEditor : Editor
{
    private SerializedProperty
        _achievementIdProp,
        _titleProp,
        _descriptionProp,
        _iconProp,
        _isUnlockedProp,
        _soundEffectProp,
        _rewardTierProp,
        _achievementListReferenceProp,
        _hasCustomGoalAmountProp,
        _goalAmountProp,
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
        _collectableReferenceProp,
        _collectableListReferenceProp,
        _minimumGoalAmountProp,
        _isUnlockAfterAchievementProp,
        _unlockAfterAchievementsProp;
    private MonoScript _monoScript;
    private void OnEnable()
    {
        _achievementIdProp = serializedObject.FindProperty("_achievementId");
        _titleProp = serializedObject.FindProperty("_title");
        _descriptionProp = serializedObject.FindProperty("_description");
        _iconProp = serializedObject.FindProperty("_icon");
        _isUnlockedProp = serializedObject.FindProperty("_isUnlocked");
        _soundEffectProp = serializedObject.FindProperty("_soundEffect");
        _rewardTierProp = serializedObject.FindProperty("_rewardTier");
        _achievementListReferenceProp = serializedObject.FindProperty("_achievementData._achievementListReference");
        _hasCustomGoalAmountProp = serializedObject.FindProperty("_achievementData._hasCustomGoalAmount");
        _goalAmountProp = serializedObject.FindProperty("_achievementData._customGoalAmount");
        _isHiddenProp = serializedObject.FindProperty("_progressionData._isHidden");
        _showProgressionProp = serializedObject.FindProperty("_progressionData._hasProgressionDisplay");
        _progressionEnumDisplayProp = serializedObject.FindProperty("_progressionData._progressionEnumDisplay");
        _completionEnumRequirementProp = serializedObject.FindProperty("_completionRequirement");
        _valueEnumTypeProp = serializedObject.FindProperty("_valueData._valueType");
        _isExactAmountProp = serializedObject.FindProperty("_valueData._isExactAmount");
        _currentIntegerAmountProp = serializedObject.FindProperty("_valueData._currentIntegerAmount");
        _goalIntegerAmountProp = serializedObject.FindProperty("_valueData._goalIntegerAmount");
        _currentFloatAmountProp = serializedObject.FindProperty("_valueData._currentFloatAmount");
        _goalFloatAmountProp = serializedObject.FindProperty("_valueData._goalFloatAmount");
        _collectableEnumRequirementProp = serializedObject.FindProperty("_collectableData._collectableRequirement");
        _collectableReferenceProp = serializedObject.FindProperty("_collectableData._collectableReference");
        _collectableListReferenceProp = serializedObject.FindProperty("_collectableData._collectableListReference");
        _minimumGoalAmountProp = serializedObject.FindProperty("_collectableData._minimumGoalAmount");
        _isUnlockAfterAchievementProp = serializedObject.FindProperty("_isUnlockedAfterAchievement");
        _unlockAfterAchievementsProp = serializedObject.FindProperty("_unlockAfterAchievements");
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
        EditorGUILayout.PropertyField(_rewardTierProp);
        EditorGUILayout.PropertyField(_soundEffectProp);
        EditorGUILayout.PropertyField(_isUnlockedProp);
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
        CompletionRequirementType completionEnumType = (CompletionRequirementType)_completionEnumRequirementProp.enumValueIndex;
        NumericValueType valueEnumType = (NumericValueType)_valueEnumTypeProp.enumValueIndex;
        CollectableRequirementType collectableEnumRequirement = (CollectableRequirementType)_collectableEnumRequirementProp.enumValueIndex;
        switch (completionEnumType)
        {
            case CompletionRequirementType.NoRequirement:
            break;
            case CompletionRequirementType.ValueRequirement:
                EditorGUILayout.PropertyField(_valueEnumTypeProp);
                EditorGUILayout.PropertyField(_isExactAmountProp);
                switch (valueEnumType)
                {
                    case NumericValueType.Integer:
                        EditorGUILayout.PropertyField(_currentIntegerAmountProp);
                        EditorGUILayout.PropertyField(_goalIntegerAmountProp);
                    break;
                    case NumericValueType.Float:
                        EditorGUILayout.PropertyField(_currentFloatAmountProp);
                        EditorGUILayout.PropertyField (_goalFloatAmountProp);
                    break;
                }
            break;
            case CompletionRequirementType.CollectableRequirement:
                EditorGUILayout.PropertyField(_collectableEnumRequirementProp);
                switch (collectableEnumRequirement)
                {
                    case CollectableRequirementType.SingleCollectable:
                        EditorGUILayout.PropertyField(_collectableReferenceProp);
                    break;
                    case CollectableRequirementType.AllCollectables:
                        EditorGUILayout.PropertyField(_collectableListReferenceProp);
                    break;
                    case CollectableRequirementType.Custom:
                        EditorGUILayout.PropertyField(_collectableListReferenceProp);
                        EditorGUILayout.PropertyField(_minimumGoalAmountProp);
                    break;
                }
            break;
            case CompletionRequirementType.AchievementRequirement:
                EditorGUILayout.PropertyField(_achievementListReferenceProp);
                EditorGUILayout.PropertyField(_hasCustomGoalAmountProp);
                if (_hasCustomGoalAmountProp.boolValue)
                {
                    EditorGUILayout.PropertyField(_goalAmountProp);
                }
            break;
        }
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_isUnlockAfterAchievementProp);
        if (_isUnlockAfterAchievementProp.boolValue)
        {
            EditorGUILayout.PropertyField(_unlockAfterAchievementsProp);
        }
        else
        {
            if (_unlockAfterAchievementsProp.isArray)
            {
                while (_unlockAfterAchievementsProp.arraySize > 0)
                    _unlockAfterAchievementsProp.DeleteArrayElementAtIndex(0);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
