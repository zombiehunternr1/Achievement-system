using System.Collections.Generic;
using UnityEngine;

public class EventPackageListenersList : MonoBehaviour
{
    [SerializeField] private List<EventPackageBase> _baseEvents;
    private void OnEnable()
    {
        UpdateRegisteryList(true);
    }
    private void OnDisable()
    {
        UpdateRegisteryList(false);
    }
    private void UpdateRegisteryList(bool isRegistering)
    {
        foreach (EventPackageBase eventDataPackage in _baseEvents)
        {
            if (isRegistering)
            {
                eventDataPackage.Registering();
            }
            else
            {
                eventDataPackage.UnRegistering();
            }
        }
    }
}
