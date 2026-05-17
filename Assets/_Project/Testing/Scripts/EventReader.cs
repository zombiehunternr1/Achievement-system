using UnityEngine;

public static class EventReader
{
    /// <summary>
    /// Returns the value of type T from the payload.
    /// Logs a warning if no data of that type was included in the dispatch.
    /// </summary>
    public static T Get<T>(EventPayload payload)
    {
        if (payload == null)
        {
            Debug.LogError("[EventReader] Payload is null.");
            return default;
        }

        if (!payload.TryGet<T>(out T value))
        {
            Debug.LogWarning($"[EventReader] No data of type '{typeof(T).Name}' found in payload. Was it included in the Raise call?");
        }

        return value;
    }

    /// <summary>
    /// Tries to read a value of type T from the payload without logging.
    /// Use this when the data is optional.
    /// </summary>
    public static bool TryGet<T>(EventPayload payload, out T value)
    {
        if (payload == null)
        {
            Debug.LogError("[EventReader] Payload is null.");
            value = default;
            return false;
        }

        return payload.TryGet<T>(out value);
    }

    /// <summary>
    /// Returns true if the payload contains data of type T.
    /// </summary>
    public static bool Has<T>(EventPayload payload)
    {
        return payload != null && payload.Has<T>();
    }
}