using System.Collections.Generic;
using UnityEngine;

public class DoubleListenersList : MonoBehaviour
{
    [SerializeField] private List<DoubleEventBase> _baseEvents;
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
        foreach (DoubleEventBase baseEvent in _baseEvents)
        {
            if (isRegistering)
            {
                baseEvent.Registering();
            }
            else
            {
                baseEvent.UnRegistering();
            }
        }
    }
}
