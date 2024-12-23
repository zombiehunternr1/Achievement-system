using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class CollectableData
{
    [SerializeField] private CollectableRequirementType _collectableRequirement;
    [SerializeField] private CollectableItem _collectableReference;
    [SerializeField] private CollectableList _collectableListReference;
    [Tooltip("This value is for the minimum amount required per collectable type in the list")]
    [SerializeField] private int _minimumGoalAmount;
    public CollectableRequirementType CollectableRequirement
    {
        get
        {
            return _collectableRequirement;
        }
    }
    public (int currentAmount, int totalAmount) GetCustomAmountDisplay()
    {
        int currentAmount = GetCurrentCollectedAmountPerCategory();
        int totalAmount = GetUniqueCategoryCount();
        return (currentAmount, totalAmount);
    }
    public (int currentAmount, int totalAmount) GetAllProgressionDisplay()
    {
        int currentAmount = 0;
        int totalAmount = 0;
        foreach (CollectableItem collectable in _collectableListReference.CollectablesList)
        {
            if (collectable.CollectableCategory == CollectibleType.None)
            {
                continue;
            }
            if (collectable.ItemAmountType == CollectionItemAmount.SingleItem)
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
        if (_collectableReference.ItemAmountType == CollectionItemAmount.SingleItem)
        {
            if (_collectableReference.IsCollected)
            {
                currentAmount = 1;
            }
            return (currentAmount, 1);
        }
        return CalculateMultiCollectableProgression(_collectableReference);
    }
    public bool IsRequirementMet(CollectableItem collectable)
    {
        if (_collectableRequirement == CollectableRequirementType.SingleCollectable)
        {
            return IsItemRequirementMet(collectable);
        }
        if (_collectableRequirement == CollectableRequirementType.AllCollectables)
        {
            return AreAllCollectablesRequirementMet();
        }
        if (_collectableRequirement == CollectableRequirementType.Custom)
        {
            return IsCustomGoalRequirementMet(_minimumGoalAmount);
        }
        return false;
    }
    public bool IsRelatedToAchievement(CollectableItem collectable)
    {
        if (_collectableRequirement == CollectableRequirementType.SingleCollectable)
        {
            if (collectable.ItemAmountType == CollectionItemAmount.SingleItem)
            {
                return _collectableReference != null && _collectableReference.IsMatchingId(collectable.CollectableId);
            }
            for (int i = 0; i < _collectableReference.MultiCollectables; i++)
            {
                if (_collectableReference.IsMatchingIdInList(i, collectable.CollectableIdFromList(i)))
                {
                    return true;
                }
            }
            return false;
        }
        return _collectableListReference.CollectablesList.Contains(collectable);
    }
    private Dictionary<CollectibleType, int> GetCollectableTotalPerCategory()
    {
        Dictionary<CollectibleType, int> collectedAmountPerCategory = new Dictionary<CollectibleType, int>();
        foreach (CollectableItem collectable in _collectableListReference.CollectablesList)
        {
            if (collectable.CollectableCategory == CollectibleType.None)
            {
                continue;
            }
            if (!collectedAmountPerCategory.TryGetValue(collectable.CollectableCategory, out int currentAmount))
            {
                currentAmount = 0;
            }
            if (collectable.ItemAmountType == CollectionItemAmount.SingleItem && collectable.IsCollected)
            {
                currentAmount++;
            }
            else
            {
                for (int i = 0; i < collectable.MultiCollectables; i++)
                {
                    if (collectable.IsCollectedFromList(i))
                    {
                        currentAmount++;
                    }
                }
            }
            collectedAmountPerCategory[collectable.CollectableCategory] = currentAmount;
        }
        return collectedAmountPerCategory;
    }
    private int GetCurrentCollectedAmountPerCategory()
    {
        int totalCategoriesMeetingGoal = 0;
        Dictionary<CollectibleType, int> collectableTotals = GetCollectableTotalPerCategory();
        foreach (int total in collectableTotals.Values)
        {
            if (total >= _minimumGoalAmount)
            {
                totalCategoriesMeetingGoal++;
            }
        }
        return totalCategoriesMeetingGoal;
    }
    private int GetUniqueCategoryCount()
    {
        HashSet<CollectibleType> uniqueCategories = new HashSet<CollectibleType>();
        for (int i = 0; i < _collectableListReference.CollectablesList.Count; i++)
        {
            CollectibleType category = _collectableListReference.CollectablesList[i].CollectableCategory;
            if (category != CollectibleType.None)
            {
                uniqueCategories.Add(category);
            }
        }
        return uniqueCategories.Count;
    }
    private (int currentAmount, int totalAmount) CalculateMultiCollectableProgression(CollectableItem collectable)
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
    private bool AreCategoriesMeetingGoal(int goalAmount)
    {
        Dictionary<CollectibleType, int> collectableTotals = GetCollectableTotalPerCategory();
        foreach (KeyValuePair<CollectibleType, int> total in collectableTotals)
        {
            if (total.Value < goalAmount)
            {
                return false;
            }
        }
        return true;
    }
    private bool AreRequirementsMetForCollectable(CollectableItem collectable)
    {
        if (collectable.ItemAmountType == CollectionItemAmount.SingleItem)
        {
            return collectable.IsCollected;
        }
        for (int i = 0; i < collectable.MultiCollectables; i++)
        {
            if (!collectable.IsCollectedFromList(i))
            {
                return false;
            }
        }
        return true;
    }
    private bool IsItemRequirementMet(CollectableItem collectable)
    {
        return AreRequirementsMetForCollectable(collectable) && _collectableReference.IsMatchingId(collectable.CollectableId);
    }
    private bool AreAllCollectablesRequirementMet()
    {
        foreach (CollectableItem collectable in _collectableListReference.CollectablesList)
        {
            if (collectable.CollectableCategory == CollectibleType.None)
            {
                continue;
            }
            if (!AreRequirementsMetForCollectable(collectable))
            {
                return false;
            }
        }
        return true;
    }
    private bool IsCustomGoalRequirementMet(int goalAmount)
    {
        return AreCategoriesMeetingGoal(goalAmount);
    }
}
