using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectableList", menuName = "Scriptable Objects/Systems/Collectables/CollectableList")]
public class CollectableList : ScriptableObject
{
    [SerializeField] private List<CollectableItem> _CollectablesList;
    public List<CollectableItem> CollectablesList
    {
        get 
        {
            return _CollectablesList;
        }
    }
}
