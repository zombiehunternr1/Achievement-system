using UnityEngine;
using System;

[Serializable]
public class ProgressionData
{
    [SerializeField] private bool _isHidden;
    [SerializeField] private bool _showProgression;
    [SerializeField] private ProgressionEnumDisplay _progressionEnumDisplay;
    public bool IsHidden
    {
        get
        {
            return _isHidden;
        }
    }
    public bool ShowProgression
    {
        get
        {
            return _showProgression;
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
