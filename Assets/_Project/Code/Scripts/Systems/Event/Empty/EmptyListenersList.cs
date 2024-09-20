using System.Collections.Generic;
using UnityEngine;

public class EmptyListenersList : MonoBehaviour
{
    [SerializeField] private List<EmptyEventBase> _baseEvents;
    public List<EmptyEventBase> BaseEvents
    {
        get
        {
            return _baseEvents;
        }
    }
    private void OnEnable()
    {
        foreach(EmptyEventBase baseEvent in _baseEvents)
        {
            baseEvent.Registering(this);
        }
    }
    private void OnDisable()
    {
        foreach (EmptyEventBase baseEvent in _baseEvents)
        {
            baseEvent.UnRegistering(this);
        }
    }
}
