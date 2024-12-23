using UnityEngine;
using System;

[Serializable]
public class ProgressionData
{
    [SerializeField] private bool _isHidden;
    [SerializeField] private bool _hasProgressionDisplay;
    [SerializeField] private ProgressionDisplayFormat _progressionEnumDisplay;
    public bool IsHidden
    {
        get
        {
            return _isHidden;
        }
    }
    public bool HasProgressionDisplay
    {
        get
        {
            return _hasProgressionDisplay;
        }
    }
    public string GetProgressionDisplayType(float currentAmount, float goalAmount)
    {
        if (goalAmount <= 0)
        {
            return "Invalid goal!";
        }
        if (_progressionEnumDisplay == ProgressionDisplayFormat.FullAmount)
        {
            return currentAmount + " / " + goalAmount;
        }
        float percentageAmount;
        percentageAmount = Mathf.InverseLerp(0, goalAmount, currentAmount) * 100;
        return percentageAmount + "%";
    }
}
