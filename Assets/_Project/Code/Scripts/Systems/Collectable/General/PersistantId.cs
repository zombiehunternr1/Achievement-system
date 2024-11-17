using UnityEngine;

[ExecuteInEditMode]
public class PersistantId : MonoBehaviour
{
    [SerializeField] private string _objectId;
    public string ObjectId
    {
        get
        {
            return _objectId;
        }
    }
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(ObjectId))
        {
            _objectId = System.Guid.NewGuid().ToString();
        }
    }
}
