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
        UpdateRegistryList(true);
    }
    private void OnDisable()
    {
        UpdateRegistryList(false);
    }
    private void UpdateRegistryList(bool isRegistering)
    {
        foreach (SingleEventBase baseEvent in _baseEvents)
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
