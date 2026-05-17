using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UniqueIdentifierAttribute))]
public class UniqueIdentifierDrawerEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var entityId = property.serializedObject.targetObject.GetEntityId();
        string assetPath = AssetDatabase.GetAssetPath(entityId);
        string uniqueID = AssetDatabase.AssetPathToGUID(assetPath);
        property.stringValue = uniqueID;
        Rect textFieldPosition = position;
        textFieldPosition.height = 16;
        EditorGUI.LabelField(position, label, new GUIContent(property.stringValue));
    }
}
