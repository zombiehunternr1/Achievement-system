using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

[CustomEditor(typeof(EventPackage))]
public class EventPackageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ShowAssignedGameObjects();
    }
    private void ShowAssignedGameObjects()
    {
        EventPackage eventPackage = (EventPackage)target;
        EditorGUILayout.LabelField("Package Key", eventPackage.PackageKey);
        List<GameObject> allGameObjects = GetAllGameObjectsInScene();
        List<string> assignedGameObjects = GetAssignedGameObjects(allGameObjects, eventPackage);
        DisplayAssignedGameObjects(assignedGameObjects, eventPackage);
    }
    private List<GameObject> GetAllGameObjectsInScene()
    {
        List<GameObject> allGameObjects = new List<GameObject>();
        foreach (GameObject scene in EditorSceneManager.GetActiveScene().GetRootGameObjects())
        {
            allGameObjects.Add(scene);
            CollectChildren(scene.transform, ref allGameObjects);
        }
        return allGameObjects;
    }
    private void CollectChildren(Transform parent, ref List<GameObject> allGameObjects)
    {
        foreach (Transform child in parent)
        {
            allGameObjects.Add(child.gameObject);
            if (child.childCount > 0)
            {
                CollectChildren(child, ref allGameObjects);
            }
        }
    }
    private List<string> GetAssignedGameObjects(List<GameObject> allGameObjects, EventPackage package)
    {
        List<string> assignedGameObjects = new List<string>();
        foreach (GameObject gameObject in allGameObjects)
        {
            bool assigned = CheckForEventPackageReference(gameObject, package);
            if (assigned)
            {
                assignedGameObjects.Add(gameObject.name);
            }
        }
        return assignedGameObjects;
    }
    private bool CheckForEventPackageReference(GameObject gameObject, EventPackage package)
    {
        foreach (MonoBehaviour monoBehaviour in gameObject.GetComponents<MonoBehaviour>())
        {
            foreach (FieldInfo field in monoBehaviour.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.FieldType == typeof(EventPackage) && field.GetValue(monoBehaviour) as EventPackage == package)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void DisplayAssignedGameObjects(List<string> assignedGameObjects, EventPackage eventPackage)
    {
        if (assignedGameObjects.Count == 0)
        {
            GUILayout.Label("<color=#ffe900><b>No GameObject(s) with this EventPackage found!<b></color>", new GUIStyle(EditorStyles.boldLabel) { richText = true });
            return;
        }
        else
        {
            GUILayout.Label("<b>Assigned in GameObject(s):</b>", new GUIStyle(EditorStyles.boldLabel) { richText = true });
            foreach (string objName in assignedGameObjects)
            {
                GUILayout.Label("- " + objName);
            }
            ShowAssignedHandlers(eventPackage);
        }
    }
    private void ShowAssignedHandlers(EventPackage eventPackage)
    {
        if (!Application.isPlaying)
        {
            GUILayout.Label("<color=#ffe900><b>Handlers are not available outside of play mode.</b></color>", new GUIStyle(EditorStyles.wordWrappedLabel) { richText = true });
            return;
        }
        GUILayout.Label("Handlers:", EditorStyles.boldLabel);
        FieldInfo handlersField = typeof(EventPackage).GetField("_eventHandlers", BindingFlags.NonPublic | BindingFlags.Instance);
        if (handlersField == null)
        {
            GUILayout.Label("No handlers field found!");
            return;
        }
        HashSet<EventPackageHandler> handlers = (HashSet<EventPackageHandler>)handlersField.GetValue(eventPackage);
        if (handlers == null || handlers.Count == 0)
        {
            GUILayout.Label("No Handlers registered!");
            return;
        }
        foreach (EventPackageHandler handler in handlers)
        {
            string handlersDescription = CreateHandlerDescription(handler);
            GUILayout.Label(handlersDescription, new GUIStyle(EditorStyles.wordWrappedLabel) { richText = true });
        }
    }
    private string CreateHandlerDescription(EventPackageHandler eventPackageHandler)
    {
        if (eventPackageHandler == null)
        {
            return "Null handler!";
        }
        FieldInfo unityEventField = typeof(EventPackageHandler).GetField("_unityEvent", BindingFlags.NonPublic | BindingFlags.Instance);
        if (unityEventField == null)
        {
            return "No Unity Event field found!";
        }
        UnityEvent<EventData> unityEvent = (UnityEvent<EventData>)unityEventField.GetValue(eventPackageHandler);
        if (unityEvent == null)
        {
            return "UnityEvent is null!";
        }
        if (unityEvent.GetPersistentEventCount() == 0)
        {
            return "No handlers attached!";
        }
        string description = string.Empty;
        Dictionary<string, List<string>> groupedMethods = new Dictionary<string, List<string>>();
        for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
        {
            string targetName;
            string methodName;
            if (unityEvent.GetPersistentTarget(i) != null)
            {
                targetName = unityEvent.GetPersistentTarget(i).name;
            }
            else
            {
                targetName = "GameObject reference is null!";
            }
            methodName = unityEvent.GetPersistentMethodName(i) ?? "Method name not found!";
            if (!groupedMethods.ContainsKey(targetName))
            {
                groupedMethods[targetName] = new List<string>();
            }
            groupedMethods[targetName].Add(methodName);
        }
        foreach (KeyValuePair<string, List<string>> entry in groupedMethods)
        {
            description += "- " + entry.Key + "\n";
            for (int i = 0; i < entry.Value.Count; i++)
            {
                description += "\t - Method: " + entry.Value[i];
                if (i < entry.Value.Count - 1)
                {
                    description += "\n";
                }
            }
            if (!IsLastEntry(groupedMethods, entry.Key))
            {
                description += "\n";
            }
        }
        return description;
    }
    private bool IsLastEntry(Dictionary<string, List<string>> groupedMethods, string currentKey)
    {
        int count = 0;
        foreach (string key in groupedMethods.Keys)
        {
            count++;
            if (key == currentKey)
            {
                break;
            }
        }
        return count == groupedMethods.Keys.Count;
    }
}