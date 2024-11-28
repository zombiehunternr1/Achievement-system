using System;
using UnityEngine;

[Serializable]
public class AchievementDTO
{
    [SerializeField] private string _title;
    [SerializeField] private bool _unlocked;
    [SerializeField] private float _currentAmount;
    public bool IsUnlocked
    {
        get
        {
            return _unlocked;
        }
    }
    public float CurrentAmount
    {
        get
        {
            return _currentAmount;
        }
    }
    public AchievementDTO(string title, bool isUnlocked, float currentAmount)
    {
        _title = title;
        _unlocked = isUnlocked;
        _currentAmount = currentAmount;
    }
    public void SetTitle(string title)
    {
        _title = title;
    }
    public void SetIsUnlockedValue(bool isUnlocked)
    {
        _unlocked = isUnlocked;
    }
    public void SetCurrentAmount(float currentAmount)
    {
        _currentAmount = currentAmount;
    }
}
