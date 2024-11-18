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
    public int MultiCollectables
    {
        get
        {
            return _multiCollectablesStatus.Count;
        }
    }
    public string CollectableId()
    {
        return _singleCollectableStatus.CollectableId;
    }
    public string CollectableIdFromList(int index)
    {
        return _multiCollectablesStatus[index].CollectableId;
    }
    public bool IsMatchingId(string id)
    {
        return _singleCollectableStatus.CollectableId.Equals(id);
    }
    public bool IsMatchingIdInList(int index, string id)
    {
        return _multiCollectablesStatus[index].CollectableId.Equals(id);
    }
    public bool IsCollected()
    {
        return _singleCollectableStatus.IsCollected;
    }
    public bool IsCollectedFromList(int index)
    {
        return _multiCollectablesStatus[index].IsCollected;
    }
    public bool IsGoalRequirementReached()
    {
        return _singleCollectableStatus.IsGoalReached;
    }
    public bool IsGoalRequirementReachedFromList(int currentIdIndex)
    {
        return _multiCollectablesStatus[currentIdIndex].IsGoalReached;
    }
    public void SetCollectableStatus(bool value)
    {
        _singleCollectableStatus.SetCollectableStatus(value);
    }
    public void SetCollectableStatusFromList(int index, bool value)
    {
        _multiCollectablesStatus[index].SetCollectableStatus(value);
    }
}
