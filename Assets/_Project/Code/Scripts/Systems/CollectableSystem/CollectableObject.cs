using UnityEngine;

public class CollectableObject : MonoBehaviour
{
    [SerializeField] private GenericEmptyEvent _updateCollectedTypeEvent;
    [SerializeField] private CollectableTypeSO _collectableType;
    public CollectableTypeSO Collectable
    {
        get
        {
            return _collectableType;
        }
    }
    public void Collect()
    {
        _collectableType.CollectCollectable = true;
        _updateCollectedTypeEvent.Invoke();
    }
}