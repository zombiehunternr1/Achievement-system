using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    [SerializeField] private long _lastUpdated;
    [SerializeField] private SerializableDictionary<string, AchievementDTO> _achievementsData;
    [SerializeField] private SerializableDictionary<string, CollectableStatusDTO> _collectablesStatusData;
    public long LastUpdated
    {
        get
        {
            return _lastUpdated;
        }
    }
    public SerializableDictionary<string, CollectableStatusDTO> CollectablesStatusData
    {
        get
        {
            return _collectablesStatusData;
        }
    }
    public SerializableDictionary<string, AchievementDTO> AchievementsData
    {
        get
        {
            return _achievementsData;
        }
    }
    public GameData()
    {
        _achievementsData = new SerializableDictionary<string, AchievementDTO>();
        _collectablesStatusData = new SerializableDictionary<string, CollectableStatusDTO>();
    }
    public int PercentageAchievementsComplete
    {
        get
        {
            if (_achievementsData.Count == 0)
            {
                return 0;
            }
            int totalUnlocked = 0;
            foreach(AchievementDTO achievementDTO in _achievementsData.Values)
            {
                if (achievementDTO.IsUnlocked)
                {
                    totalUnlocked++;
                }
            }
            return totalUnlocked * 100 / _achievementsData.Count;
        }
    }
    public int PercentageCollectionComplete
    {
        get
        {
            if (_collectablesStatusData.Count == 0)
            {
                return 0;
            }
            int totalUnlocked = 0;
            foreach(CollectableStatusDTO collectableStatus in _collectablesStatusData.Values)
            {
                if (collectableStatus.IsCollected)
                {
                    totalUnlocked++;
                }
            }
            return totalUnlocked * 100 / _collectablesStatusData.Count;
        }
    }
    public int PercentageTotalComplete
    {
        get
        {
            int totalDataCount = _achievementsData.Count + _collectablesStatusData.Count;
            if (totalDataCount == 0)
            {
                return 0;
            }
            return TotalDataCompletion * 100 / totalDataCount;
        }
    }
    private int TotalDataCompletion
    {
        get
        {
            int totalCount = 0;
            foreach (KeyValuePair<string, CollectableStatusDTO> collectableStatusPair in _collectablesStatusData)
            {
                if (collectableStatusPair.Value.IsCollected)
                {
                    totalCount++;
                }
            }
            foreach (KeyValuePair<string, AchievementDTO> achievementPair in _achievementsData)
            {
                if (achievementPair.Value.IsUnlocked)
                {
                    totalCount++;
                }
            }
            return totalCount;
        }
    }
    public void SetLastUpdated(long value)
    {
        _lastUpdated = value;
    }
    public void SetTotalAchievementsData(string idValue, string achievementTitle, bool isUnlocked, float currentAmount)
    {
        _achievementsData[idValue] = new AchievementDTO(achievementTitle, isUnlocked, currentAmount);
        _achievementsData[idValue].SetTitle(achievementTitle);
        _achievementsData[idValue].SetIsUnlockedValue(isUnlocked);
        _achievementsData[idValue].SetCurrentAmount(currentAmount);
    }
    public void SetTotalCollectablesStatusData(string iDValue, string collectableName, bool isCollected, float currentAmountValue)
    {
        _collectablesStatusData[iDValue] = new CollectableStatusDTO(collectableName, isCollected, currentAmountValue);
        _collectablesStatusData[iDValue].SetCollectableName(collectableName);
        _collectablesStatusData[iDValue].SetIsCollectedValue(isCollected);
        _collectablesStatusData[iDValue].SetIsCurrentAmount(currentAmountValue);
    }
}
