using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectableList", menuName = "Scriptable Objects/Systems/Collectables/Collectable list")]
public class CollectableList : ScriptableObject
{
    [SerializeField] private List<CollectableItem> _collectablesList;

    public List<CollectableItem> CollectablesList
    {
        get
        {
            return _collectablesList;
        }
    }
}