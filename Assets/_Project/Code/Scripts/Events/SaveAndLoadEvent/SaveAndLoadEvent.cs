using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Save&Load Data Event", menuName = "Scriptable Objects/Events/Save and Load Data Event")]
public class SaveAndLoadEvent : ScriptableObject
{
    private List<SaveAndLoadEventListener> _listeners = new List<SaveAndLoadEventListener>();

    public void RaiseSaveLoadEvent(GameData dataValue, bool boolValue)
    {
        for(int i = _listeners.Count - 1; i >= 0; i--)
        {
            _listeners[i].OnEventRaised(dataValue, boolValue);
        }
    }
    public void RegisterListener(SaveAndLoadEventListener listener)
    {
        _listeners.Add(listener);
    }
    public void UnregisterListener(SaveAndLoadEventListener listener)
    {
        _listeners.Remove(listener);
    }
}
