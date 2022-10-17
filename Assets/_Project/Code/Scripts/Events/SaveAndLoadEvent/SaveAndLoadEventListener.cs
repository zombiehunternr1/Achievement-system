using UnityEngine;
using UnityEngine.Events;

public class SaveAndLoadEventListener : MonoBehaviour
{
    [SerializeField] private SaveAndLoadEvent _saveLoadEvent;
    [SerializeField] private UnityEvent<GameData, bool> _responds;
    private void OnEnable()
    {
        _saveLoadEvent.RegisterListener(this);
    }
    private void OnDisable()
    {
        _saveLoadEvent.UnregisterListener(this);
    }
    public void OnEventRaised(GameData dataValue, bool boolvalue)
    {
        _responds.Invoke(dataValue, boolvalue);
    }
}
