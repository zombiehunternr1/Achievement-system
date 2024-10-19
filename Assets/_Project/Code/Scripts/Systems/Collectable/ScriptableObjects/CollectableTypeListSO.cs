using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectableListSO", menuName = "Scriptable Objects/Systems/Collectables/Collectable List")]
public class CollectableTypeListSO : ScriptableObject
{
    [SerializeField] private List<CollectableTypeSO> _CollectableTypes;
    public List<CollectableTypeSO> CollectableTypeList
    {
        get
        {
            return _CollectableTypes;
        }
    }
}
