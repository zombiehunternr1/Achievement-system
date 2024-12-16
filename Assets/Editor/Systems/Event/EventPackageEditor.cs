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
        EventPackage package = (EventPackage)target;
        GUILayout.Label("Assigned GameObject(s):", EditorStyles.boldLabel);
        ShowAssignedGameObjects(package);
        if (!Application.isPlaying)
        {
            GUILayout.Label("<b>Listeners are not available outside of play mode.</b>", new GUIStyle(EditorStyles.wordWrappedLabel) { richText = true });
            return;
        }
        GUILayout.Label("Listeners:", EditorStyles.boldLabel);
        FieldInfo listenersField = typeof(EventPackage).GetField("_listeners", BindingFlags.NonPublic | BindingFlags.Instance);
        if (listenersField == null)
        {
            GUILayout.Label("No listeners field found!");
            return;
        }
        HashSet<EventPackageBase> listeners = (HashSet<EventPackageBase>)listenersField.GetValue(package);
        if (listeners == null || listeners.Count == 0)
        {
            GUILayout.Label("No listeners registered!");
            return;
        }
        foreach (EventPackageBase listener in listeners)
        {
            string listenersDescription = GetListenersDescription(listener);
            GUILayout.Label(listenersDescription, new GUIStyle(EditorStyles.wordWrappedLabel) { richText = true });
        }
    }
    private void ShowAssignedGameObjects(EventPackage package)
    {
        // Find all root GameObjects in the scene
        List<GameObject> allGameObjects = GetAllGameObjectsInScene();
        // Collect assigned GameObjects that reference the EventPackage
        List<string> assignedGameObjects = GetAssignedGameObjects(allGameObjects, package);
        // Display the assigned GameObjects
        DisplayAssignedGameObjects(assignedGameObjects);
    }
    private List<GameObject> GetAllGameObjectsInScene()
    {
        List<GameObject> allGameObjects = new List<GameObject>();
        foreach (GameObject scene in EditorSceneManager.GetActiveScene().GetRootGameObjects())
        {
            allGameObjects.Add(scene);
            // Collect all child GameObjects
            CollectChildren(scene.transform, ref allGameObjects);
        }
        return allGameObjects;
    }
    private List<string> GetAssignedGameObjects(List<GameObject> allGameObjects, EventPackage package)
    {
        var assignedGameObjects = new List<string>();
        // Iterate through all GameObjects and check for components that reference the EventPackage
        foreach (GameObject gameObject in allGameObjects)
        {
            var assigned = CheckForEventPackageReference(gameObject, package);
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
    private void DisplayAssignedGameObjects(List<string> assignedGameObjects)
    {
        if (assignedGameObjects.Count == 0)
        {
            GUILayout.Label("No GameObjects with this EventPackage found!");
        }
        else
        {
            foreach (string objName in assignedGameObjects)
            {
                GUILayout.Label("- " + objName);
            }
        }
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
    private string GetListenersDescription(EventPackageBase listener)
    {
        if (listener == null)
        {
            return "Null Listener!";
        }
        FieldInfo unityEventField = typeof(EventPackageBase).GetField("_unityEvent", BindingFlags.NonPublic | BindingFlags.Instance);
        if (unityEventField == null)
        {
            return "No Unity Event field found!";
        }
        UnityEvent<EventData> unityEvent = (UnityEvent<EventData>)unityEventField.GetValue(listener);
        if (unityEvent == null)
        {
            return "UnityEvents is null!";
        }
        if (unityEvent.GetPersistentEventCount() == 0)
        {
            return "No listeners attached!";
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
            if (!string.IsNullOrEmpty(unityEvent.GetPersistentMethodName(i)))
            {
                methodName = unityEvent.GetPersistentMethodName(i);
            }
            else
            {
                methodName = "Method name not found!";
            }
            if (!groupedMethods.ContainsKey(targetName))
            {
                groupedMethods[targetName] = new List<string>();
            }
            groupedMethods[targetName].Add(methodName);
        }
        foreach (KeyValuePair<string, List<string>> entry in groupedMethods)
        {
            description += "<b>-" + entry.Key + "</b>\n";
            foreach (string method in entry.Value)
            {
                description += "\t - <b>Method:</b> " + method + "\n";
            }
        }
        return description;
    }
}
