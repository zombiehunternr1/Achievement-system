using System;
using UnityEngine;

public abstract class BaseCollectableTypeSO : ScriptableObject
{
    [SerializeField] private Type _collectableType;
    [UniqueIdentifier]
    [SerializeField] private string _collectableID;
    [SerializeField] private bool _collected;
    public string CollectableID
    {
        get
        {
            return _collectableID;
        }
    }
    public bool Collected
    {
        get
        {
            return _collected;
        }
        set
        {
            _collected = value;
        }
    }
}
