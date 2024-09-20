using System.Collections.Generic;
using UnityEngine;

public class SingleListenersList : MonoBehaviour
{
    [SerializeField] private List<SingleEventBase> _baseEvents;
    public List<SingleEventBase> BaseEvents
    {
        get
        {
            return _baseEvents;
        }
    }
    private void OnEnable()
    {
        foreach (SingleEventBase baseEvent in _baseEvents)
        {
            baseEvent.Registering(this);
        }
    }
    private void OnDisable()
    {
        foreach (SingleEventBase baseEvent in _baseEvents)
        {
            baseEvent.UnRegistering(this);
        }
    }
}
