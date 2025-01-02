using System;
using UnityEngine;

[Serializable]
public class CollectableState
{
    [SerializeField] private string _collectableId;
    [SerializeField] private bool _isCollected;
    [SerializeField] private float _currentAmount;
    [SerializeField] private float _goalAmount;
    [SerializeField] private float _increaseSpeed;
    public string CollectableId
    {
        get
        {
            return _collectableId;
        }
    }
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
    public float GoalAmount
    {
        get
        {
            return _goalAmount;
        }
    }
    public bool IsGoalReached
    {
        get
        {
            if (_currentAmount >= _goalAmount)
            {
                _currentAmount = _goalAmount;
                return true;
            }
            _currentAmount += Time.deltaTime * _increaseSpeed;
            return false;
        }
    }
    public void SetCollectableStatus(bool value)
    {
        _isCollected = value;
    }
    public void SetCurrentAmountValue(float value)
    {
        _currentAmount = value;
    }
}
