using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    [SerializeField] private long _lastUpdated;
    [SerializeField] private SerializableDictionary<string, bool> _totalAchievementsData;
    [SerializeField] private SerializableDictionary<string, bool> _totalCollectablesData;
    private List<SerializableDictionary<string, bool>> _AllData = new List<SerializableDictionary<string, bool>>();

    public long LastUpdated
    {
        get
        {
            return _lastUpdated;
        }
        set
        {
            _lastUpdated = value;
        }
    }
    public SerializableDictionary<string, bool> TotalAchievementsData
    {
        get
        {
            return _totalAchievementsData;
        }
        set
        {
            _totalAchievementsData = value;
        }
    }
    public SerializableDictionary<string, bool> TotalCollectablesData
    {
        get
        {
            return _totalCollectablesData;
        }
        set
        {
            _totalCollectablesData = value;
        }
    }
    public GameData()
    {
        _totalAchievementsData = new SerializableDictionary<string, bool>();
        _totalCollectablesData = new SerializableDictionary<string, bool>();
        _AllData.Add(_totalAchievementsData);
        _AllData.Add(_totalCollectablesData);
    }
    public int PercentageAchievementsComplete
    {
        get
        {
            int totalUnlocked = 0;
            int percentageCompleted = -1;
            foreach(bool unlocked in _totalAchievementsData.Values)
            {
                if (unlocked)
                {
                    totalUnlocked++;
                }
            }
            if(_totalAchievementsData.Count != 0)
            {
                percentageCompleted = (totalUnlocked * 100 / _totalAchievementsData.Count);
            }
            return percentageCompleted;
        }
    }
    public int PercentageCollectionComplete
    {
        get
        {
            int totalUnlocked = 0;
            int percentageCompleted = -1;
            foreach(bool collected in _totalCollectablesData.Values)
            {
                if (collected)
                {
                    totalUnlocked++;
                }
            }
            if (_totalCollectablesData.Count != 0)
            {
                percentageCompleted = (totalUnlocked * 100 / _totalCollectablesData.Count);
            }
            return percentageCompleted;
        }
    }
    public int PercentageTotalComplete
    {
        get
        {
            int percentageCompleted = -1;
            if(_totalAchievementsData.Count != 0 && _totalCollectablesData.Count != 0)
            {
                percentageCompleted = (TotalDataCompletion * 100 / (_totalAchievementsData.Count + _totalCollectablesData.Count));
            }
            return percentageCompleted;
        }
    }
    private int TotalDataCompletion
    {
        get
        {
            int totalCount = 0;
            for (int i = 0; i < _AllData.Count; i++)
            {
                foreach (var value in _AllData[i].Values)
                {
                    if (value)
                    {
                        totalCount++;
                    }
                }
            }
            return totalCount;
        }
    }
}
