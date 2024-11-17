using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CollectableObject))]
public class CollectableObjectEditor : Editor
{
    private SerializedProperty _collectableProp;
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
    private bool ShouldReplaceReference(CollectableSO reference, string currentId)
    {
        return IsReferenceAssignedToAnotherObject(reference, currentId);
    }
    private bool IsReferenceAssignedToAnotherObject(CollectableSO reference, string currentId)
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
    private bool IsReferenceAMatchWith(SerializedProperty collectableProp, CollectableSO reference, string currentId)
    {
        if (collectableProp.objectReferenceValue == reference)
        {
            CollectableSO collectable = (CollectableSO)collectableProp.objectReferenceValue;
            return collectable != null && collectable.CollectableId() == reference.CollectableId() && collectable.CollectableId() != currentId;
        }
        return false;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _collectableProp = serializedObject.FindProperty("_collectable");
        DrawDefaultInspector();
        if (_collectableProp.objectReferenceValue == null)
        {
            EditorGUILayout.HelpBox("No reference assigned yet. Please select a Collectable reference.", MessageType.Info);
        }
        else
        {
            CollectableSO currentReference = (CollectableSO)_collectableProp.objectReferenceValue;
            SerializedObject collectableSOSerialized = new SerializedObject(currentReference);
            SerializedProperty collectableItemAmountProp = collectableSOSerialized.FindProperty("_itemAmountType");
            CollectionEnumItemAmount collectionEnumType = (CollectionEnumItemAmount)collectableItemAmountProp.enumValueIndex;
            _currentObjectId = serializedObject.FindProperty("_objectId").stringValue;
            if (collectionEnumType == CollectionEnumItemAmount.SingleItem)
            {
                HandleSingleItem(currentReference, collectableSOSerialized);
            }
            else
            {
                HandleMultipleItems(currentReference, collectableSOSerialized, _currentObjectId);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
    private void HandleSingleItem(CollectableSO currentReference, SerializedObject collectableSOSerialized)
    {
        if (string.IsNullOrEmpty(currentReference.CollectableId()))
        {
            SetCollectableIDInScriptableObject(currentReference, collectableSOSerialized, _currentObjectId);
        }
        else
        {
            _currentObjectId = serializedObject.FindProperty("_objectId").stringValue;
            if (GUILayout.Button("Clear Reference"))
            {
                _collectableProp.objectReferenceValue = null;
                ClearCollectableIDInScriptableObject(currentReference, collectableSOSerialized);
            }
            if (ShouldReplaceReference(currentReference, _currentObjectId))
            {
                if (GUILayout.Button("Replace Reference"))
                {
                    _collectableProp.objectReferenceValue = currentReference;
                    SetCollectableIDInScriptableObject(currentReference, collectableSOSerialized, _currentObjectId);
                }
            }
        }
    }
    private void HandleMultipleItems(CollectableSO currentReference, SerializedObject collectableSerialized, string currentId)
    {
        SerializedProperty multiCollectablesStatusProp = collectableSerialized.FindProperty("_multiCollectablesStatus");
        int index = GetIdFromList(multiCollectablesStatusProp, currentId);
        if (index == -1)
        {
            if (GUILayout.Button("Add reference to list"))
            {
                multiCollectablesStatusProp.arraySize += 1;
                SerializedProperty newCollectableStatus = multiCollectablesStatusProp.GetArrayElementAtIndex(multiCollectablesStatusProp.arraySize - 1);
                newCollectableStatus.FindPropertyRelative("_collectableId").stringValue = currentId;
                newCollectableStatus.FindPropertyRelative("_isCollected").boolValue = false;
            }
        }
        else
        {
            if (GUILayout.Button("Remove reference from list"))
            {
                multiCollectablesStatusProp.DeleteArrayElementAtIndex(index);
            }
        }
        collectableSerialized.ApplyModifiedProperties();
    }
    private void SetCollectableIDInScriptableObject(CollectableSO collectable, SerializedObject collectableSerialized, string currentId)
    {
        SerializedProperty singleCollectableStatusProp = collectableSerialized.FindProperty("_singleCollectableStatus");
        SerializedProperty collectableIdProp = singleCollectableStatusProp.FindPropertyRelative("_collectableId");
        if (collectable.CollectableId() != currentId)
        {
            collectableIdProp.stringValue = currentId;
            collectableSerialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(collectable);
            AssetDatabase.SaveAssets();
        }
    }
    private void ClearCollectableIDInScriptableObject(CollectableSO collectable, SerializedObject collectableSerialized)
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