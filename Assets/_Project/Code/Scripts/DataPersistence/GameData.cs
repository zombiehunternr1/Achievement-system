using System;
using UnityEngine;

[Serializable]
public class GameData
{
    [SerializeField] private long _lastUpdated;
    [SerializeField] private SerializableDictionary<int, bool> _totalAchievementsData;
    [SerializeField] private SerializableDictionary<int, bool> _totalCollectablesData;

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
    public SerializableDictionary<int, bool> TotalAchievementsData
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
    public SerializableDictionary<int, bool> TotalCollectablesData
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
        _totalAchievementsData = new SerializableDictionary<int, bool>();
        _totalCollectablesData = new SerializableDictionary<int, bool>();
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
            int totalComplete = 0;
            int percentageCompleted = -1;
            foreach(bool unlocked in _totalAchievementsData.Values)
            {
                if (unlocked)
                {
                    totalComplete++;
                }
            }
            foreach(bool collected in _totalCollectablesData.Values)
            {
                if (collected)
                {
                    totalComplete++;
                }
            }
            if(_totalAchievementsData.Count != 0 && _totalCollectablesData.Count != 0)
            {
                percentageCompleted = (totalComplete * 100 / (_totalAchievementsData.Count + _totalCollectablesData.Count));
            }
            return percentageCompleted;
        }
    }
}
