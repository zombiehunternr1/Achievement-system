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
    public float GetCurrentAmount()
    {
        if (ValueEnumType == ValueEnumType.Integer)
        {
            return _currentIntegerAmount;
        }
        return _currentFloatAmount;
    }
    public (float currentAmount, float goalAmount) GetAmountDisplay()
    {
        if (_valueEnumType == ValueEnumType.Integer)
        {
            return (_currentIntegerAmount, _goalIntegerAmount);
        }
        return (_currentFloatAmount, _goalFloatAmount);
    }
    public bool IsRequirementMet()
    {
        if (ValueEnumType == ValueEnumType.Integer)
        {
            if (_isExactAmount)
            {
                return _currentIntegerAmount == _goalIntegerAmount;
            }
            if (_currentIntegerAmount >= _goalIntegerAmount)
            {
                SetToIntegerGoalAmount();
                return true;
            }
        }
        if (_isExactAmount)
        {
            return _currentFloatAmount == _goalFloatAmount;
        }
        if (_currentFloatAmount >= _goalFloatAmount)
        {
            SetToFloatGoalAmount();
            return true;
        }
        return false;
    }
    public void SetToIntegerGoalAmount()
    {
        _currentIntegerAmount = _goalIntegerAmount;
    }
    public void SetToFloatGoalAmount()
    {
        _currentFloatAmount = _goalFloatAmount;
    }
    public void SetValue(object value)
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
