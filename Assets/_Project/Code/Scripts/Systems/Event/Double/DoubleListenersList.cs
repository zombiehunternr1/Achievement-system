using System.Collections.Generic;
using UnityEngine;

public class DoubleListenersList : MonoBehaviour
{
    [SerializeField] private List<DoubleEventBase> _baseEvents;
    public List<DoubleEventBase> BaseEvents
    {
        get
        {
            return _baseEvents;
        }
    }
    private void OnEnable()
    {
        foreach (DoubleEventBase baseEvent in _baseEvents)
        {
            baseEvent.Registering(this);
        }
    }
    private void OnDisable()
    {
        foreach (DoubleEventBase baseEvent in _baseEvents)
        {
            baseEvent.UnRegistering(this);
        }
    }
}
