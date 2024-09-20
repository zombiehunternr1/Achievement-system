using System.Collections.Generic;
using UnityEngine;

public class TripleListenersList : MonoBehaviour
{
    [SerializeField] private List<TripleEventBase> _baseEvents;
    public List<TripleEventBase> BaseEvents
    {
        get
        {
            return _baseEvents;
        }
    }
    private void OnEnable()
    {
        foreach (TripleEventBase baseEvent in _baseEvents)
        {
            baseEvent.Registering(this);
        }
    }
    private void OnDisable()
    {
        foreach (TripleEventBase baseEvent in _baseEvents)
        {
            baseEvent.UnRegistering(this);
        }
    }
}
