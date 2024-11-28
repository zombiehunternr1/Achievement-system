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
            LoadCollectableStatusFromData(gameData);
        }
        else
        {
            SaveCollectableStatusToData(gameData);
        }
        _updateProgressionEvent.Invoke(gameData);
    }
    private void LoadCollectableStatusFromData(GameData gameData)
    {
        foreach (CollectableSO collectable in _allCollectablesListReference.CollectablesList)
        {
            if (collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem)
            {
                gameData.CollectablesStatusData.TryGetValue(collectable.CollectableId, out CollectableStatusDTO collectableStatesDTO);
                collectable.SetCollectableStatus(collectableStatesDTO.IsCollected);
                collectable.SetCurrentAmount(collectableStatesDTO.CurrentAmount);
            }
            else
            {
                for (int i = 0; i < collectable.MultiCollectables; i++)
                {
                    gameData.CollectablesStatusData.TryGetValue(collectable.CollectableIdFromList(i), out CollectableStatusDTO collectableStatesDTO);
                    collectable.SetCollectableStatusFromList(i, collectableStatesDTO.IsCollected);
                    collectable.SetCurrentAmountFromList(i, collectableStatesDTO.CurrentAmount);
                }
            }
        }
        _updateCollectablesStatusEvent.Invoke();
    }
    private void SaveCollectableStatusToData(GameData gameData)
    {
        List<CollectableSO>.Enumerator enumAllCollectables = _allCollectablesListReference.CollectablesList.GetEnumerator();
        string collectableId;
        bool isCollected;
        try
        {
            while (enumAllCollectables.MoveNext())
            {
                CollectableSO collectable = enumAllCollectables.Current;
                if (collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem)
                {
                    collectableId = collectable.CollectableId;
                    isCollected = collectable.IsCollected;
                    gameData.SetTotalCollectablesStatusData(collectableId, collectable.name, isCollected, collectable.CurrentAmount);
                }
                else
                {
                    for (int i = 0; i < collectable.MultiCollectables; i++)
                    {
                        collectableId = collectable.CollectableIdFromList(i);
                        isCollected = collectable.IsCollectedFromList(i);
                        gameData.SetTotalCollectablesStatusData(collectableId, collectable.name, isCollected, collectable.CurrentAmountFromList(i));
                    }
                }
            }
        }
        finally
        {
            enumAllCollectables.Dispose();
        }
    }
    #endregion
}
