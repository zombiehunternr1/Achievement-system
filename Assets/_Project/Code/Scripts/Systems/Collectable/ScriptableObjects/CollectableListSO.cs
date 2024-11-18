using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectableListSO", menuName = "Scriptable Objects/Systems/Collectables/CollectableList")]
public class CollectableListSO : ScriptableObject
{
    [SerializeField] private List<CollectableSO> _CollectablesList;
    public List<CollectableSO> CollectablesList
    {
        get 
        {
            return _CollectablesList;
        }
    }
}
