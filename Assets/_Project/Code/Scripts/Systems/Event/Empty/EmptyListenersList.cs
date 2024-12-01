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
        UpdateRegistryList(true);
    }
    private void OnDisable()
    {
        UpdateRegistryList(false);
    }
    private void UpdateRegistryList(bool isRegistering)
    {
        foreach (EmptyEventBase baseEvent in _baseEvents)
        {
            if (isRegistering)
            {
                baseEvent.Registering(this);
            }
            else
            {
                baseEvent.UnRegistering(this);
            }
        }
    }
}
