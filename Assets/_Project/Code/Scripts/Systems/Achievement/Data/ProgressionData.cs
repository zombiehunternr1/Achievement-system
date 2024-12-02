using UnityEngine;
using System;

[Serializable]
public class ProgressionData
{
    [SerializeField] private bool _isHidden;
    [SerializeField] private bool _hasProgressionDisplay;
    [SerializeField] private ProgressionEnumDisplay _progressionEnumDisplay;
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
    public ProgressionEnumDisplay ProgressionEnumDisplay
    {
        get
        {
            return _progressionEnumDisplay;
        }
    }
}
