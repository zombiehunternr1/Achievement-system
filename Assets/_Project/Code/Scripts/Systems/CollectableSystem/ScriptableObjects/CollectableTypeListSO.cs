using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectableListSO", menuName = "Scriptable Objects/Collectables/Collectable List")]
public class CollectableTypeListSO : ScriptableObject
{
    [SerializeField] private List<BaseCollectableTypeSO> _Collectables;
    public List<BaseCollectableTypeSO> CollectablesList
    {
        get
        {
            return _Collectables;
        }
    }
}
