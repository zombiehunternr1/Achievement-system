using System;
using UnityEngine;

public abstract class BaseCollectableTypeSO : ScriptableObject
{
    [SerializeField] private Type _collectableType;
    [UniqueIdentifier]
    [SerializeField] private string _collectableID;
    [SerializeField] private bool _collected;
    public string collectableId
    {
        get
        {
            return _collectableID;
        }
    }
    public bool isCollected
    {
        get
        {
            return _collected;
        }
    }
    public bool collectCollectable
    {
        set
        {
            _collected = value;
        }
    }
}
