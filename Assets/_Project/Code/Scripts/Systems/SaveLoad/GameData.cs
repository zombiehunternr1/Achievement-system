using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    [SerializeField] private long _lastUpdated;
    [SerializeField] private SerializableDictionary<string, AchievementDTO> _achievementsData;
    [SerializeField] private SerializableDictionary<string, CollectableStatusDTO> _collectablesStatus;
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
            return _collectablesStatus;
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
        _collectablesStatus = new SerializableDictionary<string, CollectableStatusDTO>();
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
            if (_collectablesStatus.Count == 0)
            {
                return 0;
            }
            int totalUnlocked = 0;
            foreach(CollectableStatusDTO collectableStatus in _collectablesStatus.Values)
            {
                if (collectableStatus.IsCollected)
                {
                    totalUnlocked++;
                }
            }
            return totalUnlocked * 100 / _collectablesStatus.Count;
        }
    }
    public int PercentageTotalComplete
    {
        get
        {
            int totalDataCount = _achievementsData.Count + _collectablesStatus.Count;
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
            foreach (KeyValuePair<string, CollectableStatusDTO> collectableStatusPair in _collectablesStatus)
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
        _collectablesStatus[iDValue] = new CollectableStatusDTO(collectableName, isCollected, currentAmountValue);
        _collectablesStatus[iDValue].SetCollectableName(collectableName);
        _collectablesStatus[iDValue].SetIsCollectedValue(isCollected);
        _collectablesStatus[iDValue].SetIsCurrentAmount(currentAmountValue);
    }
}
