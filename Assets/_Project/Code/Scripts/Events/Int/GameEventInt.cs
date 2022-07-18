using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Event Int", menuName = "Scriptable Objects/Game Events/Int")]
public class GameEventInt : ScriptableObject
{
    private List<GameEventIntListener> listeners = new List<GameEventIntListener>();

    public void RaiseIntEvent(int value)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(value);
        }
    }

    public void RegisterListener(GameEventIntListener listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(GameEventIntListener listener)
    {
        listeners.Remove(listener);
    }
}