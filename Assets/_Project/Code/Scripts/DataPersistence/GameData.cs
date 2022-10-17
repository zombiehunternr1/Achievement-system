using System;
using UnityEngine;

[Serializable]
public class GameData
{
    [SerializeField] private long _lastUpdated;
    [SerializeField] private SerializableDictionary<int, bool> _totalAchievementsData;

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
    public GameData()
    {
        _totalAchievementsData = new SerializableDictionary<int, bool>();
    }
    public int PercentageComplete
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
}
