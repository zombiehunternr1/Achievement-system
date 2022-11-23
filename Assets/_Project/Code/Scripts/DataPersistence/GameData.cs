using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    [SerializeField] private long _lastUpdated;
    [SerializeField] private SerializableDictionary<string, bool> _totalAchievementsData;
    [SerializeField] private SerializableDictionary<string, bool> _totalCollectablesData;
    private List<SerializableDictionary<string, bool>> _allData = new List<SerializableDictionary<string, bool>>();

    public long lastUpdated
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
    public SerializableDictionary<string, bool> totalAchievementsData
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
    public SerializableDictionary<string, bool> totalCollectionsData
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
        _allData.Add(_totalAchievementsData);
        _allData.Add(_totalCollectablesData);
    }
    public int percentageAchievementsComplete
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
    public int percentageCollectionComplete
    {
        get
        {
            int totalUnlocked = 0;
            int percentageCompleted = 0;
            foreach(bool collected in _totalCollectablesData.Values)
            {
                if (collected)
                {
                    totalUnlocked++;
                }
            }
            if (_totalCollectablesData.Count != 0)
            {
                percentageCompleted = totalUnlocked * 100 / _totalCollectablesData.Count;
            }
            return percentageCompleted;
        }
    }
    public int percentageTotalComplete
    {
        get
        {
            int percentageCompleted = 0;
            if(_totalAchievementsData.Count != 0 && _totalCollectablesData.Count != 0)
            {
                percentageCompleted = totalDataCompletion * 100 / (_totalAchievementsData.Count + _totalCollectablesData.Count);
            }
            return percentageCompleted;
        }
    }
    private int totalDataCompletion
    {
        get
        {
            int totalCount = 0;
            for (int i = 0; i < _allData.Count; i++)
            {
                foreach (var value in _allData[i].Values)
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
