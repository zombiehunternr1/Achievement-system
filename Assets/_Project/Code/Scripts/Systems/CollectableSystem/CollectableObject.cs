using UnityEngine;

public class CollectableObject : MonoBehaviour
{
    [SerializeField] private UpdateCollectableTypeEvent _updateCollectedTypeEvent;
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
        _collectableType.SetCollectableStatus(true);
        _updateCollectedTypeEvent.Invoke(_collectableType);
    }
}
