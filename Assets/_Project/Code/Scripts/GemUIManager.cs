using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemUIManager : MonoBehaviour
{
    [SerializeField] private List<RawImage> _gemSprites;
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
        foreach (RawImage gemsprite in _gemSprites)
        {
            CollectableObject collectableObject = gemsprite.GetComponent<CollectableObject>();
            if (collectableObject != null && collectableObject.Collectable != null)
            {
                if (collectableObject.Collectable.Collected)
                {
                    gemsprite.GetComponent<Button>().enabled = false;
                    gemsprite.color = new Color32(255, 255, 255, 255);
                }
            }
        }
    }
}
