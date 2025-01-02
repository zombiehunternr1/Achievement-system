using System;
using UnityEngine;

[Serializable]
public class CollectableStatusDTO
{
    [SerializeField] private string _collectableName;
    [SerializeField] private bool _isCollected;
    [SerializeField] private float _currentAmount;
    public bool IsCollected
    {
        get
        {
            return _isCollected;
        }
    }
    public float CurrentAmount
    {
        get
        {
            return _currentAmount;
        }
    }
    public CollectableStatusDTO(string collectableName, bool isCollected, float currentAmount)
    {
        _collectableName = collectableName;
        _isCollected = isCollected;
        _currentAmount = currentAmount;

    }
    public void SetCollectableName(string collectableName)
    {
        _collectableName = collectableName;
    }
    public void SetIsCollectedValue(bool isCollected)
    {
        _isCollected = isCollected;
    }
    public void SetIsCurrentAmount(float currentAmount)
    {
        _currentAmount = currentAmount;
    }
}