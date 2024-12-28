using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CollectableObject))]
public class CollectableObjectEditor : Editor
{
    private SerializedProperty _updateCollectedTypeEventProp, _collectableProp;
    private MonoScript _monoScript;
    private string _currentObjectId = string.Empty;
    private int GetIdFromList(SerializedProperty collectableIDList, string currentId)
    {
        for (int i = 0; i < collectableIDList.arraySize; i++)
        {
            SerializedProperty collectablStatusProp = collectableIDList.GetArrayElementAtIndex(i);
            SerializedProperty collectableIdProp = collectablStatusProp.FindPropertyRelative("_collectableId");
            if (collectableIdProp.stringValue == currentId)
            {
                return i;
            }
        }
        return -1;
    }
    private bool ShouldReplaceReference(CollectableItem reference, string currentId)
    {
        return IsReferenceAssignedToAnotherObject(reference, currentId);
    }
    private bool IsReferenceAssignedToAnotherObject(CollectableItem reference, string currentId)
    {
        CollectableObject[] allCollectables = Resources.FindObjectsOfTypeAll<CollectableObject>();
        foreach (CollectableObject collectable in allCollectables)
        {
            if (collectable == target)
            {
                continue;
            }
            SerializedObject collectableSerializedObject = new SerializedObject(collectable);
            SerializedProperty collectableProp = collectableSerializedObject.FindProperty("_collectable");
            if (IsReferenceAMatchWith(collectableProp, reference, currentId))
            {
                return true;
            }
        }
        return false;
    }
    private bool IsReferenceAMatchWith(SerializedProperty collectableProp, CollectableItem reference, string currentId)
    {
        if (collectableProp.objectReferenceValue == reference)
        {
            CollectableItem collectable = (CollectableItem)collectableProp.objectReferenceValue;
            return collectable != null && collectable.CollectableId == reference.CollectableId && collectable.CollectableId != currentId;
        }
        return false;
    }
    private void OnEnable()
    {
        _updateCollectedTypeEventProp = serializedObject.FindProperty("_updateCollectedType");
        _collectableProp = serializedObject.FindProperty("_collectable");
        _currentObjectId = serializedObject.FindProperty("_objectId").stringValue;
    }
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        _monoScript = MonoScript.FromMonoBehaviour((MonoBehaviour)target);
        EditorGUILayout.ObjectField("Script", _monoScript, typeof(MonoScript), false);
        EditorGUI.EndDisabledGroup();
        serializedObject.Update();
        EditorGUILayout.PropertyField(_updateCollectedTypeEventProp);
        EditorGUILayout.PropertyField(_collectableProp);
        if (_collectableProp.objectReferenceValue == null)
        {
            EditorGUILayout.HelpBox("No reference assigned yet. Please select a Collectable reference.", MessageType.Info);
        }
        else
        {
            CollectableItem currentReference = (CollectableItem)_collectableProp.objectReferenceValue;
            SerializedObject collectableItemSerialized = new SerializedObject(currentReference);
            if (string.IsNullOrEmpty(_currentObjectId))
            {
                CollectableObject collectableObject = serializedObject.targetObject as CollectableObject;
                collectableObject.ValidateObjectId();
            }
            SerializedProperty collectableItemAmountProp = collectableItemSerialized.FindProperty("_itemAmountType");
            CollectionItemAmount collectionEnumType = (CollectionItemAmount)collectableItemAmountProp.enumValueIndex;
            if (collectionEnumType == CollectionItemAmount.SingleItem)
            {
                HandleSingleItem(currentReference, collectableItemSerialized);
            }
            else
            {
                HandleMultipleItems(currentReference, collectableItemSerialized, _currentObjectId);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
    private void HandleSingleItem(CollectableItem currentReference, SerializedObject collectableItemSerialized)
    {
        if (string.IsNullOrEmpty(currentReference.CollectableId))
        {
            SetCollectableIDInScriptableObject(currentReference, collectableItemSerialized, _currentObjectId);
        }
        else
        {
            if (_currentObjectId.Equals(currentReference.CollectableId))
            {
                if (GUILayout.Button("Clear Reference"))
                {
                    _collectableProp.objectReferenceValue = null;
                    ClearCollectableIDInScriptableObject(currentReference, collectableItemSerialized);
                }
            }
            if (ShouldReplaceReference(currentReference, _currentObjectId))
            {
                if (GUILayout.Button("Replace Reference"))
                {
                    _collectableProp.objectReferenceValue = currentReference;
                    SetCollectableIDInScriptableObject(currentReference, collectableItemSerialized, _currentObjectId);
                }
            }
        }
    }
    private void HandleMultipleItems(CollectableItem currentReference, SerializedObject collectableItemSerialized, string currentId)
    {
        SerializedProperty multiCollectablesStatusProp = collectableItemSerialized.FindProperty("_multiCollectablesStatus");
        int index = GetIdFromList(multiCollectablesStatusProp, currentId);
        if (index == -1)
        {
            if (GUILayout.Button("Add reference to list"))
            {
                multiCollectablesStatusProp.arraySize += 1;
                SerializedProperty newCollectableStatus = multiCollectablesStatusProp.GetArrayElementAtIndex(multiCollectablesStatusProp.arraySize - 1);
                newCollectableStatus.FindPropertyRelative("_collectableId").stringValue = currentId;
                newCollectableStatus.FindPropertyRelative("_isCollected").boolValue = false;
                newCollectableStatus.FindPropertyRelative("_currentAmount").floatValue = 0;
                newCollectableStatus.FindPropertyRelative("_goalAmount").floatValue = 0;
                newCollectableStatus.FindPropertyRelative("_increaseSpeed").floatValue = 0;
            }
        }
        else
        {
            if (GUILayout.Button("Remove reference from list"))
            {
                multiCollectablesStatusProp.DeleteArrayElementAtIndex(index);
            }
        }
        collectableItemSerialized.ApplyModifiedProperties();
    }
    private void SetCollectableIDInScriptableObject(CollectableItem collectable, SerializedObject collectableSerialized, string currentId)
    {
        SerializedProperty singleCollectableStatusProp = collectableSerialized.FindProperty("_singleCollectableStatus");
        SerializedProperty collectableIdProp = singleCollectableStatusProp.FindPropertyRelative("_collectableId");
        if (collectable.CollectableId != currentId)
        {
            collectableIdProp.stringValue = currentId;
            collectableSerialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(collectable);
            AssetDatabase.SaveAssets();
        }
    }
    private void ClearCollectableIDInScriptableObject(CollectableItem collectable, SerializedObject collectableSerialized)
    {
        if (collectable != null)
        {
            SerializedProperty singleCollectableStatusProp = collectableSerialized.FindProperty("_singleCollectableStatus");
            SerializedProperty collectableIDProp = singleCollectableStatusProp.FindPropertyRelative("_collectableId");
            collectableIDProp.stringValue = string.Empty;
            collectableSerialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(collectable);
            AssetDatabase.SaveAssets();
        }
    }
}