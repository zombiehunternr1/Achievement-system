using UnityEngine;
using System;

[Serializable]
public class CollectableData
{
    [SerializeField] private CollectableEnumRequirement _collectableEnumRequirement;
    [SerializeField] private CollectableSO _collectableReference;
    [SerializeField] private CollectableListSO _collectableListReference;
    [Tooltip("This value is for the minimum amount required per collectable type in the list")]
    [SerializeField] private int _minimumGoalAmount;
    public CollectableEnumRequirement CollectableEnumRequirement
    {
        get
        {
            return _collectableEnumRequirement;
        }
    }
    public CollectableSO CollectableReference
    {
        get
        {
            return _collectableReference;
        }
    }
    public CollectableListSO CollectableListReference
    {
        get
        {
            return _collectableListReference;
        }
    }
    public int MinimumGoalAmount
    {
        get
        {
            return _minimumGoalAmount;
        }
    }
}
