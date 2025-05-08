using System.Collections.Generic;
using UnityEngine;

public class EventPackageRegistry : MonoBehaviour
{
    [SerializeField] private List<EventPackageHandler> _eventPackageHandlers;
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
        for (int i = _eventPackageHandlers.Count - 1; i >= 0; i--)
        {
            if (_eventPackageHandlers[i] == null)
            {
                _eventPackageHandlers.RemoveAt(i);
            }
        }
        foreach (EventPackageHandler eventPackageHandler in _eventPackageHandlers)
        {
            if (eventPackageHandler == null)
            {
                continue;
            }
            if (isRegistering)
            {
                eventPackageHandler.Register();
            }
            else
            {
                eventPackageHandler.Unregister();
            }
        }
    }
}
