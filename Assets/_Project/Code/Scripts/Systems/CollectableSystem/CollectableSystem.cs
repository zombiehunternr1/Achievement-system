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
            if (achievementEvent.CollectableTypeList == null || achievementEvent.CollectableTypeList.CollectablesList.Count == 0)
            {
                if (achievementEvent.collectableType == collectableType)
                {
                    _updateAchievementsEvent.Invoke(achievementEvent.AchievementId, collectedItems, null);
                }
                continue;
            }
            if(ListContainsCollectable(achievementEvent.CollectableTypeList, collectableType))
            {
                _updateAchievementsEvent.Invoke(achievementEvent.AchievementId, collectedItems, null);
            }
        }
    }
    public void ResetAllCollectibles()
    {
        foreach (CollectableTypeListSO collectableTypeList in _allcollectableListsReference.AllCollectableLists)
        {
            foreach(CollectableTypeSO collectable in collectableTypeList.CollectablesList)
            {
                collectable.SetCollectableStatus(false);
            }
        }
        _updateCollectablesStatusEvent.Invoke();
    }
    public void UpdateData(GameData data, bool isLoading)
    {
        if (isLoading)
        {
            foreach (CollectableTypeListSO collectableTypeList in _allcollectableListsReference.AllCollectableLists)
            {
                foreach(CollectableTypeSO collectableType in collectableTypeList.CollectablesList)
                {
                    data.TotalCollectionsData.TryGetValue(collectableType.CollectableId, out bool isCollected);
                    collectableType.SetCollectableStatus(isCollected);
                }
            }
            _updateCollectablesStatusEvent.Invoke();
        }
        else
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
                        if (data.TotalCollectionsData.ContainsKey(id))
                        {
                            data.TotalCollectionsData.Remove(id);
                        }
                        data.TotalCollectionsData.Add(id, value);
                    }
                }
            }
            finally
            {
                enumAllCollectablesLists.Dispose();
            }
        }
        _updateProgressionEvent.Invoke(data);
    }
}
