using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public abstract class CollectableObjectBase : MonoBehaviour
{
    [SerializeField] private string _objectId;
    private static readonly Dictionary<string, CollectableObjectBase> _collectableObjectsBaseRegistry = new Dictionary<string, CollectableObjectBase>();
    public string ObjectId
    {
        get
        {
            return _objectId;
        }
    }
    private void OnEnable()
    {
        if (string.IsNullOrEmpty(_objectId))
        {
            return;
        }
        if (!_collectableObjectsBaseRegistry.ContainsKey(_objectId))
        {
            _collectableObjectsBaseRegistry.Add(_objectId, this);
        }
    }
    private void OnDisable()
    {
        _collectableObjectsBaseRegistry.Remove(_objectId);
    }
    private void OnValidate()
    {
        ValidateObjectId();
    }
    private bool IsDuplicateId()
    {
        return _collectableObjectsBaseRegistry.TryGetValue(_objectId, out CollectableObjectBase existingObject) && existingObject != this;
    }
    public void ValidateObjectId()
    {
        if (string.IsNullOrEmpty(_objectId) || IsDuplicateId())
        {
            GenerateNewId();
        }
    }
    private void GenerateNewId()
    {
        _objectId = System.Guid.NewGuid().ToString();
    }
}
