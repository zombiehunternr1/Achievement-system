using UnityEngine;

public class CollectableObject : CollectableObjectBase
{
    [Header("Event Channels")]
    [SerializeField] private EventChannel _updateCollectedType;

    [Header("Collectable Reference")]
    [SerializeField] private CollectableItem _collectable;

    public CollectableItem Collectable => _collectable;

    public void EvaluateCollectionRequirement()
    {
        if (_collectable.ItemAmountType != CollectionItemAmount.SingleItem)
        {
            EvaluateMultiItemCollectable();
            return;
        }

        if (_collectable.CollectionType == ProcessingType.Instantly && !_collectable.IsCollected)
        {
            SetAsCollected();
            return;
        }

        if (_collectable.CollectionType == ProcessingType.Overtime && !_collectable.IsGoalRequirementReached)
        {
            SetAsCollected();
        }
    }

    private void EvaluateMultiItemCollectable()
    {
        for (int i = 0; i < _collectable.MultiCollectables; i++)
        {
            if (!_collectable.IsCollectedFromList(i) &&
                _collectable.IsMatchingIdInList(i, ObjectId) &&
                (_collectable.CollectionType == ProcessingType.Instantly ||
                 _collectable.IsGoalRequirementReachedFromList(i)))
            {
                SetAsCollectedInList(i);
                return;
            }
        }
    }

    private void SetAsCollected()
    {
        _collectable.SetCollectableStatus(true);
        EventDispatcher.Raise(_updateCollectedType, _collectable);
    }

    private void SetAsCollectedInList(int index)
    {
        _collectable.SetCollectableStatusFromList(index, true);
        EventDispatcher.Raise(_updateCollectedType, _collectable);
    }
}