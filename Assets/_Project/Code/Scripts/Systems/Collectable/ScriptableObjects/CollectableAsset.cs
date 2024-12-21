using UnityEngine;

[CreateAssetMenu(fileName = "Collectable", menuName = "Scriptable Objects/Systems/Collectables/Collectable")]
public class CollectableAsset : CollectableBase
{
    [SerializeField] private CollectibleType _collectableCategory;
    public CollectibleType CollectableCategory
    {
        get
        {
            return _collectableCategory;
        }
    }
    public void LoadCollectableStatus(GameData gameData)
    {
        if (ItemAmountType == CollectionItemAmount.SingleItem)
        {
            LoadSingleCollectableStatus(gameData);
        }
        else
        {
            LoadMultiCollectableStatus(gameData);
        }
    }
    public void SaveCollectableStatus(GameData gameData)
    {
        if (ItemAmountType == CollectionItemAmount.SingleItem)
        {
            SaveSingleCollectableStatus(gameData);
        }
        else
        {
            SaveMultiCollectableStatus(gameData);
        }
    }
    private void LoadSingleCollectableStatus(GameData gameData)
    {
        gameData.CollectablesStatusData.TryGetValue(CollectableId, out CollectableStatusDTO collectableStatusDTO);
        SetCollectableStatus(collectableStatusDTO.IsCollected);
        SetCurrentAmount(collectableStatusDTO.CurrentAmount);
    }
    private void LoadMultiCollectableStatus(GameData gameData)
    {
        for (int i = 0; i < MultiCollectables; i++)
        {
            gameData.CollectablesStatusData.TryGetValue(CollectableIdFromList(i), out CollectableStatusDTO collectableStatusDTO);
            SetCollectableStatusFromList(i, collectableStatusDTO.IsCollected);
            SetCurrentAmountFromList(i, collectableStatusDTO.CurrentAmount);
        }
    }
    private void SaveSingleCollectableStatus(GameData gameData)
    {
        gameData.SetTotalCollectablesStatusData(
            CollectableId,
            name,
            IsCollected,
            CurrentAmount
        );
    }
    private void SaveMultiCollectableStatus(GameData gameData)
    {
        for (int i = 0; i < MultiCollectables; i++)
        {
            gameData.SetTotalCollectablesStatusData(
                CollectableIdFromList(i),
                name,
                IsCollectedFromList(i),
                CurrentAmountFromList(i)
            );
        }
    }
}
