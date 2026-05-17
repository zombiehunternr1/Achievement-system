using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "EventChannel", menuName = "Events/Event Channel")]
public class EventChannel : ScriptableObject
{
    private static long _keyCounter;

    private ulong _key;
    public ulong Key
    {
        get
        {
            return _key;
        }
    }

    private readonly HashSet<EventListener> _listeners = new HashSet<EventListener>();
    private readonly List<EventListener> _snapshot = new List<EventListener>();
    private bool _isDispatching;

    private void OnEnable()
    {
        if (_key == 0)
        {
            _key = (ulong)Interlocked.Increment(ref _keyCounter);
        }
        _listeners.Clear();
        _snapshot.Clear();
        _isDispatching = false;
    }

    internal void Raise(EventPayload payload)
    {
        if (_isDispatching)
        {
            Debug.LogWarning($"[EventChannel] '{name}' raised while already dispatching. Skipping to prevent re-entrancy.");
            return;
        }

        _isDispatching = true;

        _snapshot.Clear();
        _snapshot.AddRange(_listeners);

        foreach (EventListener listener in _snapshot)
        {
            listener.Handle(payload);
        }
        _isDispatching = false;
    }

    internal void Register(EventListener listener)
    {
        _listeners.Add(listener);
    }

    internal void Unregister(EventListener listener)
    {
        _listeners.Remove(listener);
    }

#if UNITY_EDITOR
    private void OnDisable()
    {
        _listeners.Clear();
        _snapshot.Clear();
        _isDispatching = false;
    }
#endif
}