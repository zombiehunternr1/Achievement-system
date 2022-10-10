using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Event Empty", menuName = "Scriptable Objects/Events/Game Events/Empty")]
public class GameEventEmpty : ScriptableObject
{
    private List<GameEventEmptyListener> _listeners = new List<GameEventEmptyListener>();
    public void RaiseEmptyEvent()
    {
        for (int i = _listeners.Count - 1; i >= 0; i--)
        {
            _listeners[i].OnEventRaised();
        }
    }
    public void RegisterListener(GameEventEmptyListener listener)
    {
        _listeners.Add(listener);
    }
    public void UnregisterListener(GameEventEmptyListener listener)
    {
        _listeners.Remove(listener);
    }
}