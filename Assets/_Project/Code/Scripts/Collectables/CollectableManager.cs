using System.Collections.Generic;
using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    [SerializeField] private GenericEmptyEvent _saveGameEvent;
    [SerializeField] private GenericEmptyEvent _updateCollectablesEvent;
    [SerializeField] private AchievementEvent _gemCollectedAchievementEvent;
    [SerializeField] private AchievementEvent _allGemsCollectedAchievementEvent;
    [SerializeField] private UpdateProgressionEvent _updateProgressionEvent;
    [SerializeField] private List<CollectableTypeList> _allItemCollectableLists;
    public void UpdateCollectableStatus()
    {
        int collecteditems = 0;
        foreach (CollectableTypeList collectableTypeList in _allItemCollectableLists)
        {
            foreach(CollectableType collectableType in collectableTypeList.collectablesList)
            {
                if (collectableType.isCollected)
                {
                    collecteditems++;
                    _gemCollectedAchievementEvent.Invoke(_gemCollectedAchievementEvent.achievementId, collecteditems, null);
                    _allGemsCollectedAchievementEvent.Invoke(_allGemsCollectedAchievementEvent.achievementId, collecteditems, null);
                }
            }
        }
        _saveGameEvent.Invoke();
    }
    public void ResetAllCollectibles()
    {
        foreach (CollectableTypeList collectableTypeList in _allItemCollectableLists)
        {
            foreach(CollectableType collectable in collectableTypeList.collectablesList)
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
            foreach (CollectableTypeList collectableTypeList in _allItemCollectableLists)
            {
                foreach(CollectableType collectableType in collectableTypeList.collectablesList)
                {
                    data.totalCollectionsData.TryGetValue(collectableType.collectableId, out bool isCollected);
                    collectableType.collectCollectable = isCollected;
                }
            }
            _updateCollectablesEvent.Invoke();
        }
        else
        {
            List<CollectableTypeList>.Enumerator enumAllCollectablesLists = _allItemCollectableLists.GetEnumerator();
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
