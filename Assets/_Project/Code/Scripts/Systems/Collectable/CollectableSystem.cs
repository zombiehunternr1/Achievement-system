using System.Collections.Generic;
using UnityEngine;

public class CollectableSystem : MonoBehaviour
{
    [Header("Collectable references")]
    [SerializeField] private CollectableListSO _allCollectablesListReference;
    [Header("Event references")]
    [SerializeField] private EmptyEvent _saveGameEvent;
    [SerializeField] private EmptyEvent _updateCollectablesStatusEvent;
    [SerializeField] private SingleEvent _updateProgressionEvent;
    [SerializeField] private SingleEvent _checkCollectableRequestEvent;
    public void UpdateCollectableStatus(object collectableObj)
    {
        CollectableSO collectable = (CollectableSO)collectableObj;
        _checkCollectableRequestEvent.Invoke(collectable);
        _updateCollectablesStatusEvent.Invoke();
        _saveGameEvent.Invoke();
    }
    public void ResetAllCollectables()
    {
        foreach (CollectableSO collectable in _allCollectablesListReference.CollectablesList)
        {
            if (collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem)
            {
                collectable.SetCollectableStatus(false);
                collectable.SetCurrentAmount(0);
            }
            for (int i = 0; i < collectable.MultiCollectables; i++)
            {
                collectable.SetCollectableStatusFromList(i, false);
                collectable.SetCurrentAmountFromList(i, 0);
            }
        }
        _updateCollectablesStatusEvent.Invoke();
    }
    #region Saving & Loading
    public void UpdateData(object gameDataObj, object isLoadingObj)
    {
        GameData gameData = (GameData)gameDataObj;
        bool isLoading = (bool)isLoadingObj;
        if (isLoading)
        {
            LoadCollectableStatusFromGameData(gameData);
        }
        else
        {
            SaveCollectableStatusToGameData(gameData);
        }
        _updateProgressionEvent.Invoke(gameData);
    }
    private void LoadCollectableStatusFromGameData(GameData gameData)
    {
        foreach (CollectableSO collectable in _allCollectablesListReference.CollectablesList)
        {
            collectable.LoadCollectableStatus(gameData);
        }
        _updateCollectablesStatusEvent.Invoke();
    }
    private void SaveCollectableStatusToGameData(GameData gameData)
    {
        List<CollectableSO>.Enumerator enumAllCollectables = _allCollectablesListReference.CollectablesList.GetEnumerator();
        try
        {
            while (enumAllCollectables.MoveNext())
            {
                CollectableSO collectable = enumAllCollectables.Current;
                collectable.SaveCollectableStatus(gameData);
            }
        }
        finally
        {
            enumAllCollectables.Dispose();
        }
    }
    #endregion
}
