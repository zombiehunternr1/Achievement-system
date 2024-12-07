using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class CollectableData
{
    [SerializeField] private CollectableEnumRequirement _collectableEnumRequirement;
    [SerializeField] private CollectableSO _collectableReference;
    [SerializeField] private CollectableListSO _collectableListReference;
    [Tooltip("This value is for the minimum amount required per collectable type in the list")]
    [SerializeField] private int _minimumGoalAmount;
    public CollectableEnumRequirement CollectableEnumRequirement
    {
        get
        {
            return _collectableEnumRequirement;
        }
    }
    public (int currentAmount, int totalAmount) GetCustomAmountDisplay()
    {
        int currentAmount = GetCurrentCollectedAmountPerCategory(_minimumGoalAmount);
        int totalAmount = GetUniqueCategoryCount();
        return (currentAmount, totalAmount);
    }
    public (int currentAmount, int totalAmount) GetAllProgressionDisplay()
    {
        int currentAmount = 0;
        int totalAmount = 0;
        foreach (CollectableSO collectable in _collectableListReference.CollectablesList)
        {
            if (collectable.CollectableCategory == CollectableCategoryEnum.None)
            {
                continue;
            }
            if (collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem)
            {
                if (collectable.IsCollected)
                {
                    currentAmount++;
                }
                totalAmount++;
            }
            else
            {
                (int multiCurrent, int multiTotal) = CalculateMultiCollectableProgression(collectable);
                currentAmount += multiCurrent;
                totalAmount += multiTotal;
            }
        }
        return (currentAmount, totalAmount);
    }
    public (int currentAmount, int totalAmount) GetSingleProgressionDisplay()
    {
        int currentAmount = 0;
        if (_collectableReference.ItemAmountType == CollectionEnumItemAmount.SingleItem)
        {
            if (_collectableReference.IsCollected)
            {
                currentAmount = 1;
            }
            return (currentAmount, 1);
        }
        return CalculateMultiCollectableProgression(_collectableReference);
    }
    public bool IsRequirementMet(CollectableTypeSO collectable)
    {
        if (_collectableEnumRequirement == CollectableEnumRequirement.SingleCollectable)
        {
            return IsItemRequirementMet(collectable);
        }
        if (_collectableEnumRequirement == CollectableEnumRequirement.AllCollectables)
        {
            return IsAllCollectablesRequirementMet();
        }
        if (_collectableEnumRequirement == CollectableEnumRequirement.Custom)
        {
            return IsCustomGoalRequirementMet(_minimumGoalAmount);
        }
        return false;
    }
    public bool IsRelatedToAchievement(CollectableSO collectable)
    {
        if (_collectableEnumRequirement == CollectableEnumRequirement.SingleCollectable)
        {
            return _collectableReference != null && _collectableReference.IsMatchingId(collectable.CollectableId);
        }
        for (int i = 0; i < _collectableListReference.CollectablesList.Count; i++)
        {
            if (_collectableListReference.CollectablesList.Contains(collectable))
            {
                return true;
            }
        }
        return false;
    }
    private Dictionary<CollectableCategoryEnum, int> GetCollectableTotalPerCategory()
    {
        Dictionary<CollectableCategoryEnum, int> collectedAmountPerCategory = new Dictionary<CollectableCategoryEnum, int>();
        for (int i = 0; i < _collectableListReference.CollectablesList.Count; i++)
        {
            if (_collectableListReference.CollectablesList[i].CollectableCategory == CollectableCategoryEnum.None)
            {
                continue;
            }
            if (!collectedAmountPerCategory.ContainsKey(_collectableListReference.CollectablesList[i].CollectableCategory))
            {
                collectedAmountPerCategory[_collectableListReference.CollectablesList[i].CollectableCategory] = 0;
            }
        }
        foreach (CollectableSO collectable in _collectableListReference.CollectablesList)
        {
            if (collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem && collectable.IsCollected)
            {
                collectedAmountPerCategory[collectable.CollectableCategory]++;
            }
            for (int i = 0; i < collectable.MultiCollectables; i++)
            {
                if (collectable.IsCollectedFromList(i))
                {
                    collectedAmountPerCategory[collectable.CollectableCategory]++;
                }
            }
        }
        return collectedAmountPerCategory;
    }
    private int GetCurrentCollectedAmountPerCategory(int goalAmount)
    {
        Dictionary<CollectableCategoryEnum, int> collectableTotalPerCategory = GetCollectableTotalPerCategory();
        int totalCollected = 0;
        foreach (int collectedTotalPerCategory in collectableTotalPerCategory.Values)
        {
            if (collectedTotalPerCategory >= goalAmount)
            {
                totalCollected++;
            }
        }
        return totalCollected;
    }
    private int GetUniqueCategoryCount()
    {
        HashSet<CollectableCategoryEnum> uniqueCategories = new HashSet<CollectableCategoryEnum>();
        for (int i = 0; i < _collectableListReference.CollectablesList.Count; i++)
        {
            CollectableCategoryEnum category = _collectableListReference.CollectablesList[i].CollectableCategory;
            if (category == CollectableCategoryEnum.None)
            {
                continue;
            }
            if (!uniqueCategories.Contains(category))
            {
                uniqueCategories.Add(category);
            }
        }
        return uniqueCategories.Count;
    }
    private (int currentAmount, int totalAmount) CalculateMultiCollectableProgression(CollectableSO collectable)
    {
        int currentAmount = 0;
        int totalAmount = collectable.MultiCollectables;

        for (int i = 0; i < totalAmount; i++)
        {
            if (collectable.IsCollectedFromList(i))
            {
                currentAmount++;
            }
        }
        return (currentAmount, totalAmount);
    }
    private bool IsItemRequirementMet(CollectableTypeSO collectable)
    {
        if (_collectableReference.ItemAmountType == CollectionEnumItemAmount.SingleItem)
        {
            return _collectableReference.IsMatchingId(collectable.CollectableId) && _collectableReference.IsCollected;
        }
        for (int i = 0; i < _collectableReference.MultiCollectables; i++)
        {
            if (!_collectableReference.IsMatchingIdInList(i, collectable.CollectableIdFromList(i)) ||
                !_collectableReference.IsCollectedFromList(i))
            {
                return false;
            }
        }
        return true;
    }
    private bool IsAllCollectablesRequirementMet()
    {
        for (int i = 0; i < _collectableListReference.CollectablesList.Count; i++)
        {
            if (_collectableListReference.CollectablesList[i].CollectableCategory == CollectableCategoryEnum.None)
            {
                continue;
            }
            if (_collectableListReference.CollectablesList[i].ItemAmountType == CollectionEnumItemAmount.SingleItem)
            {
                if (!_collectableListReference.CollectablesList[i].IsCollected)
                {
                    return false;
                }
            }
            for (int j = 0; j < _collectableListReference.CollectablesList[i].MultiCollectables; j++)
            {
                if (!_collectableListReference.CollectablesList[i].IsCollectedFromList(j))
                {
                    return false;
                }
            }
        }
        return true;
    }
    private bool IsCustomGoalRequirementMet(int goalAmount)
    {
        if (goalAmount < 0)
        {
            return false;
        }
        Dictionary<CollectableCategoryEnum, int> collectableTotalPerCategory = GetCollectableTotalPerCategory();
        foreach (int collectedTotalPerCategory in collectableTotalPerCategory.Values)
        {
            if (collectedTotalPerCategory < goalAmount)
            {
                return false;
            }
        }
        return true;
    }
}
