using System.Collections.Generic;
using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    [SerializeField] private GenericEmptyEvent _saveGameEvent;
    [SerializeField] private AchievementEvent _gemCollectedEvent;
    [SerializeField] private AchievementEvent _allGemsCollectedEvent;
    [SerializeField] private List<CollectableTypeList> _AllItemCollectableLists;
    public void UpdateCollectableStatus()
    {
        int collecteditems = 0;
        foreach (CollectableTypeList collectableTypeList in _AllItemCollectableLists)
        {
            foreach(CollectableType collectableType in collectableTypeList.CollectablesList)
            {
                if (collectableType.Collected)
                {
                    collecteditems++;
                    _gemCollectedEvent.Invoke(_gemCollectedEvent.AchievementID, collecteditems, null);
                    _allGemsCollectedEvent.Invoke(_allGemsCollectedEvent.AchievementID, collecteditems, null);
                    _saveGameEvent.Invoke();
                }
            }
        }
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
                    collectableType.Collected = isCollected;
                }
            }
        }
        else
        {
            foreach (CollectableTypeList collectableTypeList in _AllItemCollectableLists)
            {
                foreach (CollectableType collectableType in collectableTypeList.CollectablesList)
                {
                    if (data.TotalCollectablesData.ContainsKey(collectableType.CollectableID))
                    {
                        data.TotalCollectablesData.Remove(collectableType.CollectableID);
                    }
                    data.TotalCollectablesData.Add(collectableType.CollectableID, collectableType.Collected);
                }
            }
        }
    }
}
