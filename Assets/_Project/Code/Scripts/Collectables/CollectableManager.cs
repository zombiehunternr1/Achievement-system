using System.Collections.Generic;
using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    [SerializeField] private GenericEmptyEvent _saveGameEvent;
    [SerializeField] private GenericEmptyEvent _updateCollectablesEvent;
    [SerializeField] private UpdateProgressionEvent _updateProgressionEvent;
    [SerializeField] private AchievementEvent _gemCollectedAchievementEvent;
    [SerializeField] private AchievementEvent _allGemsCollectedAchievementEvent;
    [SerializeField] private List<CollectableTypeList> _AllItemCollectableLists;
    public void UpdateCollectableStatus()
    {
        int collecteditems = 0;
        foreach (CollectableTypeList collectableTypeList in _AllItemCollectableLists)
        {
            foreach(CollectableType collectableType in collectableTypeList.CollectablesList)
            {
                if (collectableType.IsCollected)
                {
                    collecteditems++;
                    _gemCollectedAchievementEvent.Invoke(_gemCollectedAchievementEvent.AchievementID, collecteditems, null);
                    _allGemsCollectedAchievementEvent.Invoke(_allGemsCollectedAchievementEvent.AchievementID, collecteditems, null);
                }
            }
        }
        _saveGameEvent.Invoke();
    }
    public void UpdateData(GameData data, bool isLoading)
    {
        if (isLoading)
        {
            foreach (CollectableTypeList collectableTypeList in _AllItemCollectableLists)
            {
                foreach(CollectableType collectableType in collectableTypeList.CollectablesList)
                {
                    data.TotalCollectablesData.TryGetValue(collectableType.CollectableID, out bool isCollected);
                    collectableType.CollectCollectable = isCollected;
                }
            }
            _updateCollectablesEvent.Invoke();
        }
        else
        {
            int collectableListsIndex = _AllItemCollectableLists.Count;
            int currentListIndex = 0;
            while (currentListIndex < collectableListsIndex)
            {
                int collectablesInList = _AllItemCollectableLists[currentListIndex].CollectablesList.Count;
                int currentCollectableIndex = 0;
                while (currentCollectableIndex < collectablesInList)
                {
                    if (data.TotalCollectablesData.ContainsKey(_AllItemCollectableLists[currentListIndex].CollectablesList[currentCollectableIndex].CollectableID))
                    {
                        data.TotalCollectablesData.Remove(_AllItemCollectableLists[currentListIndex].CollectablesList[currentCollectableIndex].CollectableID);
                    }
                    data.TotalCollectablesData.Add(_AllItemCollectableLists[currentListIndex].CollectablesList[currentCollectableIndex].CollectableID, _AllItemCollectableLists[currentListIndex].CollectablesList[currentCollectableIndex].IsCollected);
                    Debug.Log(_AllItemCollectableLists[currentListIndex].CollectablesList[currentCollectableIndex]);
                    currentCollectableIndex++;
                }
                currentListIndex++;
            }
        }
        _updateProgressionEvent.Invoke(data);
    }
}
