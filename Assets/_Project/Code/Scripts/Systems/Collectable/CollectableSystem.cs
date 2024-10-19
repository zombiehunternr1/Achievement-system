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
    [SerializeField] private SingleEvent _checkCollectableRequestEvent;
    public void UpdateCollectableStatus(object collectableTypeObj)
    {
        CollectableTypeSO collectableType = (CollectableTypeSO)collectableTypeObj;
        foreach (CollectableTypeListSO collectableTypeList in _allcollectableListsReference.AllCollectableLists)
        {
            if (collectableTypeList.CollectableTypeList.Contains(collectableType))
            {
                _checkCollectableRequestEvent.Invoke(collectableType);
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
        foreach (CollectableTypeSO collectable in collectableTypeList.CollectableTypeList)
        {
            collectable.SetCollectableStatus(false);
        }
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
    private void LoadCollectableStatusFromData(GameData data)
    {
        foreach (CollectableTypeListSO collectableTypeList in _allcollectableListsReference.AllCollectableLists)
        {
            foreach (CollectableTypeSO collectableType in collectableTypeList.CollectableTypeList)
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
                List<CollectableTypeSO>.Enumerator enumCurrentCollectableList = enumAllCollectablesLists.Current.CollectableTypeList.GetEnumerator();

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
    #endregion
}
