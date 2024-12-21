using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectableUIHolder : MonoBehaviour
{
    [SerializeField] private List<RawImage> _collectableSprites;
    private void OnEnable()
    {
        RefreshUI();
    }
    public void UpdateCollectedStatus()
    {
        RefreshUI();
    }
    private void RefreshUI()
    {
        foreach (RawImage collectableSprite in _collectableSprites)
        {
            CollectableObject collectableObject = collectableSprite.GetComponent<CollectableObject>();
            if (collectableObject == null || collectableObject.Collectable == null)
            {
                continue;
            }
            bool isCollected = false;
            CollectableItem collectable = collectableObject.Collectable;
            if (collectable.ItemAmountType == CollectionItemAmount.SingleItem)
            {
                isCollected = collectable.IsCollected;
            }
            else if (collectable.ItemAmountType == CollectionItemAmount.MultipleItems)
            {
                isCollected = IsMultiCollectableCollected(collectableObject, collectable);
            }
            UpdateCollectableSprite(collectableSprite, isCollected);
        }
    }
    private bool IsMultiCollectableCollected(CollectableObject collectableObject, CollectableItem collectable)
    {
        for (int i = 0; i < collectable.MultiCollectables; i++)
        {
            if (collectable.IsCollectedFromList(i) && 
                collectableObject.ObjectId == collectable.CollectableIdFromList(i))
            {
                return true;
            }
        }
        return false;
    }
    private void UpdateCollectableSprite(RawImage collectableSprite, bool isCollected)
    {
        Button button = collectableSprite.GetComponent<Button>();
        button.enabled = !isCollected;
        if (isCollected)
        {
            collectableSprite.color = new Color32(255, 255, 255, 255);
        }
        else
        {
            collectableSprite.color = new Color32(255, 255, 255, 125);
        }
    }
}
