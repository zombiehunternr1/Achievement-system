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
    public bool IsRequirementMet(CollectableSO collectable)
    {
        if (_collectableEnumRequirement == CollectableEnumRequirement.SingleCollectable)
        {
            return IsItemRequirementMet(collectable);
        }
        if (_collectableEnumRequirement == CollectableEnumRequirement.AllCollectables)
        {
            return AreAllCollectablesRequirementMet();
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
            if (collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem)
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
    private Dictionary<CollectableCategoryEnum, int> GetCollectableTotalPerCategory()
    {
        Dictionary<CollectableCategoryEnum, int> collectedAmountPerCategory = new Dictionary<CollectableCategoryEnum, int>();

        foreach (var collectable in _collectableListReference.CollectablesList)
        {
            if (collectable.CollectableCategory == CollectableCategoryEnum.None)
            {
                continue;
            }
            if (!collectedAmountPerCategory.ContainsKey(collectable.CollectableCategory))
            {
                collectedAmountPerCategory[collectable.CollectableCategory] = 0;
            }
            if (collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem && collectable.IsCollected)
            {
                collectedAmountPerCategory[collectable.CollectableCategory]++;
            }
            else
            {
                for (int i = 0; i < collectable.MultiCollectables; i++)
                {
                    if (collectable.IsCollectedFromList(i))
                    {
                        collectedAmountPerCategory[collectable.CollectableCategory]++;
                    }
                }
            }
        }
        return collectedAmountPerCategory;
    }
    private int GetCurrentCollectedAmountPerCategory(int goalAmount)
    {
        AreCategoriesMeetingGoal(goalAmount, out int totalCategoriesMeetingGoal);
        return totalCategoriesMeetingGoal;
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
    private bool AreCategoriesMeetingGoal(int goalAmount, out int totalCategoriesMeetingGoal)
    {
        totalCategoriesMeetingGoal = 0;
        Dictionary<CollectableCategoryEnum, int> collectableTotals = GetCollectableTotalPerCategory();
        foreach (int total in  collectableTotals.Values)
        {
            if (total >= goalAmount)
            {
                totalCategoriesMeetingGoal++;
            }
            else if (goalAmount > 0)
            {
                return false;
            }
        }
        return goalAmount <= 0 || totalCategoriesMeetingGoal > 0;
    }
    private bool AreRequirementsMetForCollectable(CollectableSO collectable)
    {
        if (collectable.ItemAmountType == CollectionEnumItemAmount.SingleItem)
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
    private bool IsItemRequirementMet(CollectableSO collectable)
    {
        return AreRequirementsMetForCollectable(collectable) && _collectableReference.IsMatchingId(collectable.CollectableId);
    }
    private bool AreAllCollectablesRequirementMet()
    {
        foreach (CollectableSO collectable in _collectableListReference.CollectablesList)
        {
            if (collectable.CollectableCategory == CollectableCategoryEnum.None)
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
        return AreCategoriesMeetingGoal(goalAmount, out _);
    }
}
