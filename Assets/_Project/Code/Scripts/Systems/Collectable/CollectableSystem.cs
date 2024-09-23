using System.Collections.Generic;
using UnityEngine;

public class CollectableSystem : MonoBehaviour
{
    [Header("Collectable references")]
    [SerializeField] private CollectableListHolder _allcollectableListsReference;
    [Header("Event references")]
    [SerializeField] private EmptyEvent _saveGameEvent;
    [SerializeField] private EmptyEvent _updateCollectablesStatusEvent;
    [SerializeField] private SingleEvent _updateProgressionEvent;
    [SerializeField] private TripleEvent _checkCollectableRequestEvent;
    private int CountCollectedItems(CollectableTypeListSO collectableTypeList)
    {
        int collectedItems = 0;
        for(int i = 0; i < collectableTypeList.CollectablesList.Count; i++)
        {
            if (collectableTypeList.CollectablesList[i].IsCollected)
            {
                collectedItems++;
            }
        }
        return collectedItems;
    }
    public void UpdateCollectableStatus(object collectableTypeObj)
    {
        CollectableTypeSO collectableType = (CollectableTypeSO)collectableTypeObj;
        int collectedItemsAmount = 0;
        foreach (CollectableTypeListSO collectableTypeList in _allcollectableListsReference.AllCollectableLists)
        {
            if (collectableTypeList.CollectablesList.Contains(collectableType))
            {
                collectedItemsAmount += CountCollectedItems(collectableTypeList);
                _checkCollectableRequestEvent.Invoke(collectableTypeList, collectableType, collectedItemsAmount);
            }
        }
        _updateCollectablesStatusEvent.Invoke();
        _saveGameEvent.Invoke();
    }
    public void ResetAllCollectibles()
    {
        foreach (CollectableTypeListSO collectableTypeList in _allcollectableListsReference.AllCollectableLists)
        {
            ResetCollectablesInList(collectableTypeList);
        }
        _updateCollectablesStatusEvent.Invoke();
    }
    private void ResetCollectablesInList(CollectableTypeListSO collectableTypeList)
    {
        foreach (CollectableTypeSO collectable in collectableTypeList.CollectablesList)
        {
            collectable.SetCollectableStatus(false);
        }
    }
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
    private void LoadCollectableStatusFromData(GameData data)
    {
        foreach (CollectableTypeListSO collectableTypeList in _allcollectableListsReference.AllCollectableLists)
        {
            foreach (CollectableTypeSO collectableType in collectableTypeList.CollectablesList)
            {
                data.TotalCollectionsData.TryGetValue(collectableType.CollectableId, out bool isCollected);
                collectableType.SetCollectableStatus(isCollected);
            }
        }
        _updateCollectablesStatusEvent.Invoke();
    }
    private void SaveCollectableStatusToData(GameData data)
    {
        List<CollectableTypeListSO>.Enumerator enumAllCollectablesLists = _allcollectableListsReference.AllCollectableLists.GetEnumerator();
        try
        {
            while (enumAllCollectablesLists.MoveNext())
            {
                List<BaseCollectableTypeSO>.Enumerator enumCurrentCollectableList = enumAllCollectablesLists.Current.CollectablesList.GetEnumerator();

                while (enumCurrentCollectableList.MoveNext())
                {
                    string id = enumCurrentCollectableList.Current.CollectableId;
                    bool value = enumCurrentCollectableList.Current.IsCollected;
                    data.SetTotalCollectionsData(id, value);
                }
            }
        }
        finally
        {
            enumAllCollectablesLists.Dispose();
        }
    }
}
