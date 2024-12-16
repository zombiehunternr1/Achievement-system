using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CustomEditor(typeof(EventPackage))]
public class EventPackageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
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
        EventPackage package = (EventPackage)target;
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
