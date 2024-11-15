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
    public List<CollectableSO> SingleItems
    {
        get
        {
            List<CollectableSO> singleItems = new List<CollectableSO>();
            for (int i = 0; i < _CollectablesList.Count; i++)
            {
                if (_CollectablesList[i].ItemAmountType == CollectionEnumItemAmount.SingleItem)
                {
                    singleItems.Add(_CollectablesList[i]);
                }
            }
            return singleItems;
        }
    }
    public List<CollectableSO> MultipleItems
    {
        get
        {
            List<CollectableSO> mutipleItems = new List<CollectableSO>();
            for (int i = 0; i < _CollectablesList.Count; i++)
            {
                if (_CollectablesList[i].ItemAmountType == CollectionEnumItemAmount.MultipleItems)
                {
                    mutipleItems.Add(_CollectablesList[i]);
                }
            }
            return mutipleItems;
        }
    }
}
