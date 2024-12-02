using System;
using UnityEngine;

[Serializable]
public class ValueData
{
    [SerializeField] private ValueEnumType _valueEnumType;
    [SerializeField] private bool _isExactAmount;
    [SerializeField] private int _currentIntegerAmount;
    [SerializeField] private int _goalIntegerAmount;
    [SerializeField] private float _currentFloatAmount;
    [SerializeField] private float _goalFloatAmount;
    public ValueEnumType ValueEnumType
    {
        get
        {
            return _valueEnumType;
        }
    }
    public bool IsExactAmount
    {
        get
        {
            return _isExactAmount;
        }
    }
    public int CurrentIntegerAmount
    {
        get
        {
            return _currentIntegerAmount;
        }
    }
    public int GoalIntegerAmount
    {
        get
        {
            return _goalIntegerAmount;
        }
    }
    public float CurrentFloatAmount
    {
        get
        {
            return _currentFloatAmount;
        }
    }
    public float GoalFloatAmount
    {
        get
        {
            return _goalFloatAmount;
        }
    }
    public void SetToIntegerGoalAmount()
    {
        _currentIntegerAmount = _goalIntegerAmount;
    }
    public void SetToFloatGoalAmount()
    {
        _currentFloatAmount = _goalFloatAmount;
    }
    public void SetNewValue(object value)
    {
        if (_valueEnumType == ValueEnumType.Integer)
        {
            _currentIntegerAmount = Convert.ToInt32(value);
        }
        else
        {
            _currentFloatAmount = Convert.ToSingle(value);
        }
    }
}
