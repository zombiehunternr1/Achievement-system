using System.Collections.Generic;
using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    [SerializeField] private GenericEmptyEvent _saveGameEvent;
    [SerializeField] private GenericEmptyEvent _updateCollectablesEvent;
    [SerializeField] private UpdateProgressionEvent _updateProgressionEvent;
    [SerializeField] private AchievementEvent _allCollectablesCollectedEvent;
    [SerializeField] private List<AchievementEvent> _gemCollectableEvents;
    [SerializeField] private List<CollectableTypeListSO> _allItemCollectableLists;
    public void UpdateCollectableStatus()
    {
        int collecteditems = 0;
        foreach (CollectableTypeListSO collectableTypeList in _allItemCollectableLists)
        {
            foreach(CollectableTypeSO collectableType in collectableTypeList.collectablesList)
            {
                if (collectableType.isCollected)
                {
                    collecteditems++;
                    foreach (AchievementEvent achievementEvent in _gemCollectableEvents)
                    {
                        achievementEvent.Invoke(achievementEvent.achievementId, collecteditems, null);
                    }
                    _allCollectablesCollectedEvent.Invoke(_allCollectablesCollectedEvent.achievementId, collecteditems, null);
                }
            }
        }
        _saveGameEvent.Invoke();
    }
    public void ResetAllCollectibles()
    {
        foreach (CollectableTypeListSO collectableTypeList in _allItemCollectableLists)
        {
            foreach(CollectableTypeSO collectable in collectableTypeList.collectablesList)
            {
                collectable.collectCollectable = false;
            }
        }
        _updateCollectablesEvent.Invoke();
    }
    public void UpdateData(GameData data, bool isLoading)
    {
        if (isLoading)
        {
            foreach (CollectableTypeListSO collectableTypeList in _allItemCollectableLists)
            {
                foreach(CollectableTypeSO collectableType in collectableTypeList.collectablesList)
                {
                    data.totalCollectionsData.TryGetValue(collectableType.collectableId, out bool isCollected);
                    collectableType.collectCollectable = isCollected;
                }
            }
            _updateCollectablesEvent.Invoke();
        }
        else
        {
            List<CollectableTypeListSO>.Enumerator enumAllCollectablesLists = _allItemCollectableLists.GetEnumerator();
            try
            {
                while (enumAllCollectablesLists.MoveNext())
                {
                    List<BaseCollectableTypeSO>.Enumerator enumCurrentCollectableList = enumAllCollectablesLists.Current.collectablesList.GetEnumerator();
                    while (enumCurrentCollectableList.MoveNext())
                    {
                        string id = enumCurrentCollectableList.Current.collectableId;
                        bool value = enumCurrentCollectableList.Current.isCollected;
                        if (data.totalCollectionsData.ContainsKey(id))
                        {
                            data.totalCollectionsData.Remove(id);
                        }
                        data.totalCollectionsData.Add(id, value);
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
