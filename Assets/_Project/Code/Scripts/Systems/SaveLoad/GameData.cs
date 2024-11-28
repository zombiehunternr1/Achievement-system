using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    [SerializeField] private long _lastUpdated;
    [SerializeField] private SerializableDictionary<string, bool> _totalAchievementsData;
    [SerializeField] private SerializableDictionary<string, CollectableStatusDTO> _collectablesStatus;
    [SerializeField] private SerializableDictionary<string, float> _currentValueData;
    private List<SerializableDictionary<string, bool>> _allData = new List<SerializableDictionary<string, bool>>();
    public long LastUpdated
    {
        get
        {
            return _lastUpdated;
        }
    }
    public SerializableDictionary<string, bool> TotalAchievementsData
    {
        get
        {
            return _totalAchievementsData;
        }
    }
    public SerializableDictionary <string, float> CurrentValueData
    {
        get
        {
            return _currentValueData;
        }
    }
    public SerializableDictionary<string, CollectableStatusDTO> CollectablesStatus
    {
        get
        {
            return _collectablesStatus;
        }
    }
    public GameData()
    {
        _totalAchievementsData = new SerializableDictionary<string, bool>();
        _currentValueData = new SerializableDictionary<string, float>();
        _collectablesStatus = new SerializableDictionary<string, CollectableStatusDTO>();
        _allData.Add(_totalAchievementsData);
    }
    public int PercentageAchievementsComplete
    {
        get
        {
            int totalUnlocked = 0;
            int percentageCompleted = 0;
            foreach(bool unlocked in _totalAchievementsData.Values)
            {
                if (unlocked)
                {
                    totalUnlocked++;
                }
            }
            if(_totalAchievementsData.Count != 0)
            {
                percentageCompleted = totalUnlocked * 100 / _totalAchievementsData.Count;
            }
            return percentageCompleted;
        }
    }
    public int PercentageCollectionComplete
    {
        get
        {
            int totalUnlocked = 0;
            int percentageCompleted = 0;
            foreach(CollectableStatusDTO collectableStatus in _collectablesStatus.Values)
            {
                if (collectableStatus.IsCollected)
                {
                    totalUnlocked++;
                }
            }
            if (_collectablesStatus.Count != 0)
            {
                percentageCompleted = totalUnlocked * 100 / _collectablesStatus.Count;
            }
            return percentageCompleted;
        }
    }
    public int PercentageTotalComplete
    {
        get
        {
            int percentageCompleted = 0;
            if(_totalAchievementsData.Count != 0 && _collectablesStatus.Count != 0)
            {
                percentageCompleted = TotalDataCompletion * 100 / (_totalAchievementsData.Count + _collectablesStatus.Count);
            }
            return percentageCompleted;
        }
    }
    private int TotalDataCompletion
    {
        get
        {
            int totalCount = 0;
            List<bool> allValues = new();
            foreach (var dataList in _allData)
            {
                allValues.AddRange(dataList.Values);
            }
            foreach (bool value in allValues)
            {
                if (value)
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
    public void SetTotalAchievementsData(string idValue, bool boolValue)
    {
        _totalAchievementsData[idValue] = boolValue;
    }
    public void SetCurrentValueData(string iDValue, float value)
    {
        _currentValueData[iDValue] = value;
    }
    public void SetTotalCollectablesStatus(string iDValue, string collectableName, bool isCollected, float currentAmountValue)
    {
        _collectablesStatus[iDValue] = new CollectableStatusDTO(collectableName, isCollected, currentAmountValue);
        _collectablesStatus[iDValue].SetCollectableName(collectableName);
        _collectablesStatus[iDValue].SetIsCollectedValue(isCollected);
        _collectablesStatus[iDValue].SetIsCurrentAmount(currentAmountValue);
    }
}
