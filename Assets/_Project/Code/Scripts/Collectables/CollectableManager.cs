using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class CollectableManager : MonoBehaviour
{
    [SerializeField] private GenericEmptyEvent _saveGameEvent;
    [SerializeField] private GenericEmptyEvent _updateCollectablesEvent;
    [SerializeField] private UpdateProgressionEvent _updateProgressionEvent;
    [SerializeField] private AchievementEvent _gemCollectedAchievementEvent;
    [SerializeField] private AchievementEvent _allGemsCollectedAchievementEvent;
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
            foreach (CollectableTypeList collectableTypeList in _allItemCollectableLists)
            {
                foreach(CollectableType collectableType in collectableTypeList.collectablesList)
                {
                    if (data.totalCollectionsData.ContainsKey(collectableType.collectableId))
                    {
                        data.totalCollectionsData.Remove(collectableType.collectableId);
                    }
                    data.totalCollectionsData.Add(collectableType.collectableId, collectableType.isCollected);
                }
            }
        }
        _updateProgressionEvent.Invoke(data);
    }
}
