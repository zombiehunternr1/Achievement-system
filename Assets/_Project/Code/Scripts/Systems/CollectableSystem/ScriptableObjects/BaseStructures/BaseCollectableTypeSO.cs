using UnityEngine;

public abstract class BaseCollectableTypeSO : ScriptableObject
{
    [UniqueIdentifier]
    [SerializeField] private string _collectableID;
    [SerializeField] private bool _isCollected;
    public string CollectableId
    {
        get
        {
            return _collectableID;
        }
    }
    public bool IsCollected
    {
        get
        {
            return _isCollected;
        }
    }
    public bool CollectCollectable
    {
        set
        {
            _isCollected = value;
        }
    }
}