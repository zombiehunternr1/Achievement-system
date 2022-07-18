using UnityEngine;
using UnityEngine.Events;

public class GameEventFloatListener : MonoBehaviour
{
    [SerializeField] private GameEventFloat floatevent;
    [SerializeField] private UnityEvent<float> respondse;

    private void OnEnable()
    {
        floatevent.RegisterListener(this);
    }

    private void OnDisable()
    {
        floatevent.UnregisterListener(this);
    }

    public void OnEventRaised(float value)
    {
        respondse.Invoke(value);
    }
}