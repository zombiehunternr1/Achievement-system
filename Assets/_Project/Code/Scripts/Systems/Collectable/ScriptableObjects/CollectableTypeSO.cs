using System.Collections.Generic;
using UnityEngine;

public class CollectableTypeSO : ScriptableObject
{
    [SerializeField] private CollectionEnumType _collectionType;
    [SerializeField] private CollectionEnumItemAmount _itemAmountType;
    [SerializeField] private CollectableStatus _singleCollectableStatus;
    [SerializeField] private List<CollectableStatus> _multiCollectablesStatus;
    public CollectionEnumType CollectionType
    {
        get
        {
            return _collectionType;
        }
    }
    public CollectionEnumItemAmount ItemAmountType
    {
        get
        {
            return _itemAmountType;
        }
    }
    public CollectableStatus SingleCollectableStatus
    {
        get
        {
            return _singleCollectableStatus;
        }
    }
    public List<CollectableStatus> MultiCollectableStatus
    {
        get
        {
            return _multiCollectablesStatus;
        }
    }
    public bool IsGoalRequirementReached()
    {
        return _itemAmountType == CollectionEnumItemAmount.SingleItem && _singleCollectableStatus.IsGoalReached;
    }
    public bool IsGoalRequirementReached(int currentIdIndex)
    {
        return _itemAmountType == CollectionEnumItemAmount.MultipleItems && _multiCollectablesStatus[currentIdIndex].IsGoalReached;
    }
    public void SetCollectableStatus(bool value)
    {
        _singleCollectableStatus.SetCollectableStatus(value);
    }
}
