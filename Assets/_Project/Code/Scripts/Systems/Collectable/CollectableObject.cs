using UnityEngine;

public class CollectableObject : MonoBehaviour
{
    [SerializeField] private SingleEvent _updateCollectedTypeEvent;
    [SerializeField] private CollectableSO _collectable;
    public CollectableTypeSO Collectable
    {
        get
        {
            return _collectable;
        }
    }
    public void CheckCollectionRequirement()
    {
        string objectId = gameObject.GetInstanceID().ToString();
        if (_collectable.CollectionType == CollectionEnumType.Instantly)
        {
            CheckInstantly(objectId);
        }
        else
        {
            CheckOverTime(objectId);
        }
    }
    private void CheckInstantly(string objectId)
    {
        if (_collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem)
        {
            if (!_collectable.SingleCollectableStatus.IsCollected)
            {
                Collect();
                return;
            }
        }
        CheckMultiCollectables(objectId, false);
    }
    private void CheckOverTime(string objectId)
    {
        if (_collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem)
        {
            if (!_collectable.SingleCollectableStatus.IsCollected && _collectable.IsGoalRequirementReached())
            {
                Collect();
                return;
            }
        }
        CheckMultiCollectables(objectId, true);
    }
    private void CheckMultiCollectables(string objectId, bool isGoalRequired)
    {
        for (int i = 0; i < _collectable.MultiCollectableStatus.Count; i++)
        {
            if (_collectable.MultiCollectableStatus[i].IsCollected)
            {
                continue;
            }
            if (_collectable.MultiCollectableStatus[i].CollectableId == objectId && (!isGoalRequired || _collectable.IsGoalRequirementReached(i)))
            {
                _collectable.MultiCollectableStatus[i].SetCollectableStatus(true);
                _updateCollectedTypeEvent.Invoke(_collectable);
                return;
            }
        }
    }
    private void Collect()
    {
        _collectable.SetCollectableStatus(true);
        _updateCollectedTypeEvent.Invoke(_collectable);
    }
}
