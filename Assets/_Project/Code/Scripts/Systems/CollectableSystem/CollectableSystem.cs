using System.Collections.Generic;
using UnityEngine;

public class CollectableSystem : MonoBehaviour
{
    [SerializeField] private GenericEmptyEvent _saveGameEvent;
    [SerializeField] private GenericEmptyEvent _updateCollectablesStatusEvent;
    [SerializeField] private UpdateProgressionEvent _updateProgressionEvent;
    [SerializeField] private UpdateAchievementsEvent _updateAchievementsEvent;
    [SerializeField] private CollectableListHolder _allcollectableListsReference;
    private bool ListContainsCollectable(CollectableTypeListSO collectableTypeList, CollectableTypeSO collectableType)
    {
        foreach (CollectableTypeSO collectableTypeFromList in collectableTypeList.CollectablesList)
        {
            if (collectableTypeFromList == collectableType)
            {
                return true;
            }
        }
        return false;
    }
    private int CountCollectedItems(CollectableTypeListSO collectableTypeList)
    {
        int collectedItems = 0;
        foreach (CollectableTypeSO collectableTypeFromList in collectableTypeList.CollectablesList)
        {
            if (collectableTypeFromList.IsCollected)
            {
                collectedItems++;
            }
        }
        return collectedItems;
    }
    public void UpdateCollectableStatus(CollectableTypeSO collectableType)
    {
        int collectedItems = 0;
        foreach (CollectableTypeListSO collectableTypeList in _allcollectableListsReference.AllCollectableLists)
        {
            if (ListContainsCollectable(collectableTypeList, collectableType))
            {
                collectedItems += CountCollectedItems(collectableTypeList);
                UpdateAchievementsForCollectable(collectableType, collectedItems);
            }
        }
        _updateCollectablesStatusEvent.Invoke();
        _saveGameEvent.Invoke();
    }
    private void UpdateAchievementsForCollectable(CollectableTypeSO collectableType, int collectedItems)
    {
        foreach (AchievementReferenceHolderSO achievementEvent in _updateAchievementsEvent.achievementReferences)
        {
            bool hasCollectableTypeList = achievementEvent.CollectableTypeList != null;
            bool hasCollectables = hasCollectableTypeList && achievementEvent.CollectableTypeList.CollectablesList.Count > 0;
            if (hasCollectables && !ListContainsCollectable(achievementEvent.CollectableTypeList, collectableType))
            {
                continue;
            }
            if (!hasCollectables && achievementEvent.collectableType != collectableType)
            {
                continue;
            }
            _updateAchievementsEvent.Invoke(achievementEvent.AchievementId, collectedItems, null);
        }
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
    public void UpdateData(GameData data, bool isLoading)
    {
        if (isLoading)
        {
            UpdateCollectableStatusFromData(data);
        }
        else
        {
            SaveCollectableStatusToData(data);
        }
        _updateProgressionEvent.Invoke(data);
    }
    private void UpdateCollectableStatusFromData(GameData data)
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
