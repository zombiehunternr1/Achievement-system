using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectableUIManager : MonoBehaviour
{
    [SerializeField] private List<RawImage> _collectableSprites;
    private void OnEnable()
    {
        UpdateImages();
    }
    public void UpdateCollectedStatus()
    {
        UpdateImages();
    }
    private void UpdateImages()
    {
        foreach (RawImage collectableSprit in _collectableSprites)
        {
            CollectableObject collectableObject = collectableSprit.GetComponent<CollectableObject>();
            if (collectableObject != null && collectableObject.Collectable != null)
            {
                if (collectableObject.Collectable.Collected)
                {
                    collectableSprit.GetComponent<Button>().enabled = false;
                    collectableSprit.color = new Color32(255, 255, 255, 255);
                }
                else
                {
                    collectableSprit.GetComponent<Button>().enabled = true;
                    collectableSprit.color = new Color32(255, 255, 255, 125);
                }
            }
        }
    }
}
