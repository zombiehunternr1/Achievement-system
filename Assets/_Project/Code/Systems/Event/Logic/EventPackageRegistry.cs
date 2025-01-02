using System.Collections.Generic;
using UnityEngine;

public class EventPackageRegistry : MonoBehaviour
{
    [SerializeField] private List<EventPackageHandler> _eventPackageHandlers;
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
        foreach (EventPackageHandler eventPackageHandler in _eventPackageHandlers)
        {
            if (isRegistering)
            {
                eventPackageHandler.Registering();
            }
            else
            {
                eventPackageHandler.UnRegistering();
            }
        }
    }
}
