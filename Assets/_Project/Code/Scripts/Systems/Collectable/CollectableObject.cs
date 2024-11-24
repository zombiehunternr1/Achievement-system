using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CollectableObject : MonoBehaviour
{
    [SerializeField] private SingleEvent _updateCollectedTypeEvent;
    [SerializeField] private CollectableSO _collectable;
    [SerializeField] private string _objectId;
    
    private static readonly Dictionary<string, CollectableObject> _collectableObjectsRegistry = new Dictionary<string, CollectableObject>();
    private void OnEnable()
    {
        if (string.IsNullOrEmpty(_objectId))
        {
            return;
        }
        if (!_collectableObjectsRegistry.ContainsKey(_objectId))
        {
            _collectableObjectsRegistry.Add(_objectId, this);
        }
    }
    private void OnDisable()
    {
        _collectableObjectsRegistry.Remove(_objectId);
    }
    private void OnValidate()
    {
        CheckObjectId();
    }
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
    private bool IsDuplicateId()
    {
        if (_collectableObjectsRegistry.TryGetValue(_objectId, out CollectableObject existingObject))
        {
            return existingObject != this;
        }
        return false;
    }
    public void CheckObjectId()
    {
        if (string.IsNullOrEmpty(_objectId) || IsDuplicateId())
        {
            GenerateNewId();
        }
    }
    public void EvaluateCollectionRequirement()
    {
        if (_collectable.ItemAmountType != CollectionEnumItemAmount.SingleItem)
        {
            EvaluateMultiItemCollection();
            return;
        }
        if (_collectable.CollectionType == CollectionEnumType.Instantly && !_collectable.IsCollected())
        {
            Collect();
            return;
        }
        if (_collectable.CollectionType == CollectionEnumType.Overtime && !_collectable.IsGoalRequirementReached())
        {
            Collect();
        }
    }
    private void GenerateNewId()
    {
        _objectId = System.Guid.NewGuid().ToString();
    }
    private void EvaluateMultiItemCollection()
    {
        for (int i = 0; i < _collectable.MultiCollectables; i++)
        {
            if (!_collectable.IsCollectedFromList(i) &&
                _collectable.IsMatchingIdInList(i, _objectId) &&
                (_collectable.CollectionType == CollectionEnumType.Instantly ||
                 _collectable.IsGoalRequirementReachedFromList(i)))
            {
                MarkCollectedInList(i);
                return;
            }
        }
    }
    private void Collect()
    {
        _collectable.SetCollectableStatus(true);
        _updateCollectedTypeEvent.Invoke(_collectable);
    }
    private void MarkCollectedInList(int index)
    {
        _collectable.SetCollectableStatusFromList(index, true);
        _updateCollectedTypeEvent.Invoke(_collectable);
    }
}
