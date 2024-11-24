using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(CollectableSO))]
public class CollectableSOEditor : Editor
{
    private SerializedProperty
       _collectableIdProp,
       _isCollectedProp,
        _collectableCategoryProp,
       _collectionTypeProp,
       _itemAmountTypeProp,
       _currentAmountProp,
       _goalAmountProp,
       _increaseSpeedProp,
       _singleCollectableStatusProp,
       _multiCollectablesStatusProp;
    private MonoScript _monoScript;
    private ReorderableList _multiCollectablesList;
    private void OnEnable()
    {
        _collectableCategoryProp = serializedObject.FindProperty("_collectableCategory");
        _collectionTypeProp = serializedObject.FindProperty("_collectionType");
        _itemAmountTypeProp = serializedObject.FindProperty("_itemAmountType");
        _singleCollectableStatusProp = serializedObject.FindProperty("_singleCollectableStatus");
        _multiCollectablesStatusProp = serializedObject.FindProperty("_multiCollectablesStatus");
        _collectableIdProp = _singleCollectableStatusProp.FindPropertyRelative("_collectableId");
        _isCollectedProp = _singleCollectableStatusProp.FindPropertyRelative("_isCollected");
        _multiCollectablesList = new ReorderableList(serializedObject, _multiCollectablesStatusProp, true, true, true, true);
        _multiCollectablesList.drawHeaderCallback = DrawHeaderElement;
        _multiCollectablesList.drawElementCallback = DrawBackElement;
    }
    private void DrawHeaderElement(Rect rect)
    {
        EditorGUI.LabelField(rect, "Collectables Status");
    }
    private void DrawBackElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = _multiCollectablesStatusProp.GetArrayElementAtIndex(index);
        rect.y += 2;
        float labelWidth = 90;
        float checkboxWidth = 75;
        float isCollectedWidth = 20;
        float availableWidth = rect.width;
        float fieldWidth = Mathf.Max(availableWidth - (labelWidth + checkboxWidth + isCollectedWidth + 15), 100f);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), "Collectable ID:");
        EditorGUI.PropertyField(new Rect(rect.x + labelWidth, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight),
                                element.FindPropertyRelative("_collectableId"), GUIContent.none);
        EditorGUI.EndDisabledGroup();
        EditorGUI.LabelField(new Rect(rect.x + labelWidth + fieldWidth + 5, rect.y, checkboxWidth, EditorGUIUtility.singleLineHeight), "Is Collected:");
        EditorGUI.PropertyField(new Rect(rect.x + labelWidth + fieldWidth + checkboxWidth + 5, rect.y, isCollectedWidth, EditorGUIUtility.singleLineHeight),
                                element.FindPropertyRelative("_isCollected"), GUIContent.none);
        CollectionEnumType collectionEnumType = (CollectionEnumType)_collectionTypeProp.enumValueIndex;
        switch (collectionEnumType)
        {
            case CollectionEnumType.Overtime:
                rect.y += EditorGUIUtility.singleLineHeight + 4;
                _multiCollectablesList.elementHeight = EditorGUIUtility.singleLineHeight * 5 + 10;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth + 10, EditorGUIUtility.singleLineHeight), "Current Amount:");
                EditorGUI.PropertyField(new Rect(rect.x + (labelWidth + 10), rect.y, fieldWidth, EditorGUIUtility.singleLineHeight),
                                        element.FindPropertyRelative("_currentAmount"), GUIContent.none);
                rect.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth + 10, EditorGUIUtility.singleLineHeight), "Goal Amount:");
                EditorGUI.PropertyField(new Rect(rect.x + (labelWidth + 10), rect.y, fieldWidth, EditorGUIUtility.singleLineHeight),
                                        element.FindPropertyRelative("_goalAmount"), GUIContent.none);
                rect.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth + 10, EditorGUIUtility.singleLineHeight), "Increase Speed:");
                EditorGUI.PropertyField(new Rect(rect.x + (labelWidth + 10), rect.y, fieldWidth, EditorGUIUtility.singleLineHeight),
                                        element.FindPropertyRelative("_increaseSpeed"), GUIContent.none);
                break;
            case CollectionEnumType.Instantly:
                _multiCollectablesList.elementHeight = EditorGUIUtility.singleLineHeight;
                break;
        }
    }
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        _monoScript = MonoScript.FromScriptableObject((ScriptableObject)target);
        EditorGUILayout.ObjectField("Script", _monoScript, typeof(MonoScript), false);
        EditorGUI.EndDisabledGroup();
        serializedObject.Update();
        EditorGUILayout.PropertyField(_collectableCategoryProp);
        EditorGUILayout.PropertyField(_collectionTypeProp);
        EditorGUILayout.PropertyField(_itemAmountTypeProp);
        CollectionEnumType collectionEnumType = (CollectionEnumType)_collectionTypeProp.enumValueIndex;
        CollectionEnumItemAmount collectionItemAmount = (CollectionEnumItemAmount)_itemAmountTypeProp.enumValueIndex;
        switch (collectionEnumType)
        {
            case CollectionEnumType.Instantly:
                switch (collectionItemAmount)
                {
                    case CollectionEnumItemAmount.SingleItem:
                        _multiCollectablesStatusProp.arraySize = 0;
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.PropertyField(_collectableIdProp, new GUIContent("Collectable ID"));
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.PropertyField(_isCollectedProp);
                        break;
                    case CollectionEnumItemAmount.MultipleItems:
                        _collectableIdProp.stringValue = string.Empty;
                        _isCollectedProp.boolValue = false;
                        _multiCollectablesList.DoLayoutList();
                        break;
                }
                break;
            case CollectionEnumType.Overtime:
                switch (collectionItemAmount)
                {
                    case CollectionEnumItemAmount.SingleItem:
                        _multiCollectablesStatusProp.arraySize = 0;
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.PropertyField(_collectableIdProp, new GUIContent("Collectable ID"));
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.PropertyField(_isCollectedProp);
                        _currentAmountProp = _singleCollectableStatusProp.FindPropertyRelative("_currentAmount");
                        _goalAmountProp = _singleCollectableStatusProp.FindPropertyRelative("_goalAmount");
                        _increaseSpeedProp = _singleCollectableStatusProp.FindPropertyRelative("_increaseSpeed");
                        EditorGUILayout.PropertyField(_currentAmountProp);
                        EditorGUILayout.PropertyField(_goalAmountProp);
                        EditorGUILayout.PropertyField(_increaseSpeedProp);
                        break;
                    case CollectionEnumItemAmount.MultipleItems:
                        _collectableIdProp.stringValue = string.Empty;
                        _isCollectedProp.boolValue = false;
                        _multiCollectablesList.DoLayoutList();
                        break;
                }
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}