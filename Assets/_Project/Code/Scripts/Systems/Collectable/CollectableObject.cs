using UnityEngine;

public class CollectableObject : MonoBehaviour
{
    [SerializeField] private SingleEvent _updateCollectedTypeEvent;
    [SerializeField] private CollectableSO _collectable;
    [SerializeField, HideInInspector] private string _objectId = System.Guid.NewGuid().ToString();
    public CollectableTypeSO Collectable
    {
        get
        {
            return _collectable;
        }
    }
    public string ObjectId
    {
        get
        {
            return _objectId;
        }
    }
    public void CheckCollectionRequirement()
    {
        if (_collectable.CollectionType == CollectionEnumType.Instantly)
        {
            CheckInstantly(_objectId);
        }
        else
        {
            CheckOverTime(_objectId);
        }
    }
    private void CheckInstantly(string objectId)
    {
        if (_collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem && !_collectable.IsCollected())
        {
            Collect();
            return;
        }
        CheckMultiCollectables(objectId, false);
    }
    private void CheckOverTime(string objectId)
    {
        if (_collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem && _collectable.IsGoalRequirementReached())
        {
            Collect();
            return;
        }
        CheckMultiCollectables(objectId, true);
    }
    private void CheckMultiCollectables(string objectId, bool isGoalRequired)
    {
        for (int i = 0; i < _collectable.MultiCollectables; i++)
        {
            if (_collectable.IsCollected(i))
            {
                continue;
            }
            if (_collectable.IsMatchingId(i, objectId) && (!isGoalRequired || _collectable.IsGoalRequirementReached(i)))
            {
                Collect(i);
                return;
            }
        }
    }
    private void Collect()
    {
        _collectable.SetCollectableStatus(true);
        _updateCollectedTypeEvent.Invoke(_collectable);
    }
    private void Collect(int index)
    {
        _collectable.SetCollectableStatus(index, true);
        _updateCollectedTypeEvent.Invoke(_collectable);
    }
}
