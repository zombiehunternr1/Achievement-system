using System.Collections;
using UnityEngine;

public class HoldButtonUIController : MonoBehaviour
{
    [SerializeField] private CollectableObject _collectableObjectReference;
    public void HoldButtonDown(bool isHolding)
    {
        if (isHolding)
        {
            StartCoroutine(EvaluateWhileHolding());
        }
        else
        {
            StopAllCoroutines();
        }
    }
    private IEnumerator EvaluateWhileHolding()
    {
        while (true)
        {
            _collectableObjectReference.EvaluateCollectionRequirement();
            yield return null;
        }
    }
}
