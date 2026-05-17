using System.Collections.Generic;
using UnityEngine;

public class EventListenerGroup : MonoBehaviour
{
    [SerializeField] private List<EventListener> _listeners;

    private void OnEnable()
    {
        SetRegistration(true);
    }

    private void OnDisable()
    {
        SetRegistration(false);
    }

    private void SetRegistration(bool register)
    {
        if (_listeners == null)
        {
            return;
        }

        foreach (EventListener listener in _listeners)
        {
            if (listener == null)
            {
                continue;
            }
            if (register)
            {
                listener.Register(this);
            }
            else
            {
                listener.Unregister();
            }
        }
    }
}