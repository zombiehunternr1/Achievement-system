using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CollectableListHolder", menuName ="Scriptable Objects/Systems/Collectables/Collectable List Holder")]
public class CollectableListHolder : ScriptableObject
{
    [SerializeField] private List<CollectableTypeListSO> _collectableTypeListReferences;
    public List<CollectableTypeListSO> AllCollectableLists
    {
        get
        {
            return _collectableTypeListReferences;
        }
    }
}
