using System.Collections.Generic;
using UnityEngine;

public static class EventPayloadPool
{
    private const int MaxPoolSize = 64;

    private static readonly Stack<EventPayload> _pool = new Stack<EventPayload>(16);

    public static EventPayload Get(ulong key)
    {
        EventPayload payload;
        if (_pool.Count > 0)
        {
            payload = _pool.Pop();
        }
        else
        {
            payload = new EventPayload();
        }

        payload.OnAcquire(key);
        return payload;
    }

    public static void Release(EventPayload payload)
    {
        if (payload == null || payload.IsInPool || _pool.Count >= MaxPoolSize)
        {
            return;
        }
        payload.OnRelease();
        _pool.Push(payload);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void OnReset() => _pool.Clear();
}