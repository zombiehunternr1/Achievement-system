using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Event Float", menuName = "Scriptable Objects/Events/Game Events/Float")]
public class GameEventFloat : ScriptableObject
{
    private List<GameEventFloatListener> listeners = new List<GameEventFloatListener>();

    public void RaiseFloatEvent(float value)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(value);
        }
    }

    public void RegisterListener(GameEventFloatListener listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(GameEventFloatListener listener)
    {
        listeners.Remove(listener);
    }
}