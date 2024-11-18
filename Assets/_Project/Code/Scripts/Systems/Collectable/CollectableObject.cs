using UnityEngine;

public class CollectableObject : MonoBehaviour
{
    [SerializeField] private SingleEvent _updateCollectedTypeEvent;
    [SerializeField] private CollectableSO _collectable;
    [SerializeField] private string _objectId = System.Guid.NewGuid().ToString();
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
            CheckCollectInstantly();
        }
        else
        {
            CheckCollectOverTime();
        }
    }
    private void CheckCollectInstantly()
    {
        if (_collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem && !_collectable.IsCollected())
        {
            Collect();
            return;
        }
        CheckCollectableInList(false);
    }
    private void CheckCollectOverTime()
    {
        if (_collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem && _collectable.IsGoalRequirementReached())
        {
            Collect();
            return;
        }
        CheckCollectableInList(true);
    }
    private void CheckCollectableInList(bool isGoalRequired)
    {
        for (int i = 0; i < _collectable.MultiCollectables; i++)
        {
            if (_collectable.IsCollectedFromList(i))
            {
                continue;
            }
            if (_collectable.IsMatchingIdInList(i, _objectId) && (!isGoalRequired || _collectable.IsGoalRequirementReachedFromList(i)))
            {
                CollectFromList(i);
                return;
            }
        }
    }
    private void Collect()
    {
        _collectable.SetCollectableStatus(true);
        _updateCollectedTypeEvent.Invoke(_collectable);
    }
    private void CollectFromList(int index)
    {
        _collectable.SetCollectableStatusFromList(index, true);
        _updateCollectedTypeEvent.Invoke(_collectable);
    }
}
