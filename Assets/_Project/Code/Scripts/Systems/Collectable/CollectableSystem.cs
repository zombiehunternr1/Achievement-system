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
    public void UpdateCollectableStatus(object collectableV2Obj)
    {
        CollectableSO collectableV2 = (CollectableSO)collectableV2Obj;
        _checkCollectableRequestEvent.Invoke(collectableV2);
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
            }
            foreach (CollectableStatus collectableStatus in collectable.MultiCollectableStatus)
            {
                collectableStatus.SetCollectableStatus(false);
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
        foreach (CollectableSO collectableV2 in _allCollectablesListReference.CollectablesList)
        {
            if (collectableV2.ItemAmountType == CollectionEnumItemAmount.SingleItem)
            {
                gameData.TotalCollectionsData.TryGetValue(collectableV2.SingleCollectableStatus.CollectableId, out bool isCollected);
                collectableV2.SetCollectableStatus(isCollected);
            }
            for (int i = 0; i < collectableV2.MultiCollectableStatus.Count; i++)
            {
                gameData.TotalCollectionsData.TryGetValue(collectableV2.MultiCollectableStatus[i].CollectableId, out bool isCollected);
                collectableV2.MultiCollectableStatus[i].SetCollectableStatus(isCollected);
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
                if (enumAllCollectables.Current.ItemAmountType == CollectionEnumItemAmount.SingleItem)
                {
                    collectableId = enumAllCollectables.Current.SingleCollectableStatus.CollectableId;
                    isCollected = enumAllCollectables.Current.SingleCollectableStatus.IsCollected;
                    gameData.SetTotalCollectionsData(collectableId, isCollected);
                }
                else
                {
                    for (int i = 0; i < enumAllCollectables.Current.MultiCollectableStatus.Count; i++)
                    {
                        collectableId = enumAllCollectables.Current.MultiCollectableStatus[i].CollectableId;
                        isCollected = enumAllCollectables.Current.MultiCollectableStatus[i].IsCollected;
                        gameData.SetTotalCollectionsData(collectableId, isCollected);
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
