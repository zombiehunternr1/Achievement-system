using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Event Empty", menuName = "Scriptable Objects/Game Events/Empty")]
public class GameEventEmpty : ScriptableObject
{
    private List<GameEventEmptyListener> listeners = new List<GameEventEmptyListener>();

    public void RaiseEmptyEvent()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
        }
    }

    public void RegisterListener(GameEventEmptyListener listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(GameEventEmptyListener listener)
    {
        listeners.Remove(listener);
    }
}