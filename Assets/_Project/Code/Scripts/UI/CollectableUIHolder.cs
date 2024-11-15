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
            if (collectableObject.Collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem)
            {
                isCollected = collectableObject.Collectable.SingleCollectableStatus.IsCollected;
            }
            else if (collectableObject.Collectable.ItemAmountType == CollectionEnumItemAmount.MultipleItems)
            {
                isCollected = IsMultiCollectableCollected(collectableObject);
            }
            UpdateCollectableSprite(collectableSprite, isCollected);
        }
    }
    private bool IsMultiCollectableCollected(CollectableObject collectableObject)
    {
        foreach (CollectableStatus collectableStatus in collectableObject.Collectable.MultiCollectableStatus)
        {
            if (collectableStatus.IsCollected && collectableObject.gameObject.GetInstanceID().ToString() == collectableStatus.CollectableId)
                return true;
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
