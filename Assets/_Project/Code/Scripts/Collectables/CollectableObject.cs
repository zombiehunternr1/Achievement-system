using UnityEngine;

public class CollectableObject : MonoBehaviour
{
    [SerializeField] private GenericEmptyEvent _updateCollectedTypeEvent;
    [SerializeField] private CollectableType _collectableType;
    public CollectableType Collectable
    {
        get
        {
            return _collectableType;
        }
    }
    public void Collect()
    {
        _collectableType.Collected = true;
        _updateCollectedTypeEvent.Invoke();
    }
}
