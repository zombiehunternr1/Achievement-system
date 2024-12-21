using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectableList", menuName = "Scriptable Objects/Systems/Collectables/CollectableList")]
public class CollectableList : ScriptableObject
{
    [SerializeField] private List<CollectableAsset> _CollectablesList;
    public List<CollectableAsset> CollectablesList
    {
        get 
        {
            return _CollectablesList;
        }
    }
}
