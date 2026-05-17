using UnityEngine;

public static class EventDispatcher
{
    public static void Raise(EventChannel channel, params object[] values)
    {
        if (!Validate(channel)) return;

        EventPayload payload = EventPayloadPool.Get(channel.Key);
        try
        {
            if (values != null && values.Length > 0)
            {
                foreach (object value in values)
                {
                    payload.SetRaw(value);
                }
            }

            channel.Raise(payload);
        }
        finally
        {
            EventPayloadPool.Release(payload);
        }
    }

    private static bool Validate(EventChannel channel)
    {
        if (channel == null)
        {
            Debug.LogError("[EventDispatcher] Channel cannot be null.");
            return false;
        }

        if (channel.Key == 0)
        {
            Debug.LogError($"[EventDispatcher] Channel '{channel.name}' has not been initialised. Was OnEnable called?");
            return false;
        }

        return true;
    }
}