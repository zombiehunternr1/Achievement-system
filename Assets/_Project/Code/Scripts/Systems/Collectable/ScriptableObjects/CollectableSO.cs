using UnityEngine;

[CreateAssetMenu(fileName = "CollectableSO", menuName = "Scriptable Objects/Systems/Collectables/Collectable")]
public class CollectableSO : CollectableTypeSO
{
    [SerializeField] private CollectableCategoryEnum _collectableCategory;
    public CollectableCategoryEnum CollectableCategory
    {
        get
        {
            return _collectableCategory;
        }
    }
}
