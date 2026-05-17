using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EventListener
{
    [SerializeField] private EventChannel _channel;
    [SerializeField] private UnityEvent<EventPayload> _response;

    public void Register(UnityEngine.Object context = null)
    {
        if (_channel == null)
        {
            string contextName = "Unknown Object";

            if (context != null)
            {
                contextName = context.name;
            }

            Debug.LogError(
                "[EventListener] Channel reference is missing on " + contextName,
                context
            );
            return;
        }

        _channel.Register(this);
    }

    public void Unregister()
    {
        if (_channel == null)
        {
            return;
        }
        _channel.Unregister(this);
    }

    internal void Handle(EventPayload payload)
    {
        if (_response != null)
        {
            _response.Invoke(payload);
        }
    }
}