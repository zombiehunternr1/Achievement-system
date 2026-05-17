using System.Collections.Generic;
using UnityEngine;

public class CollectableSystem : MonoBehaviour
{
    [Header("Collectable References")]
    [SerializeField] private CollectableList _allCollectablesListReference;

    [Header("Event Channels")]
    [SerializeField] private EventChannel _checkCollectableRequest;
    [SerializeField] private EventChannel _updateCollectablesStatus;
    [SerializeField] private EventChannel _updateProgression;
    [SerializeField] private EventChannel _saveGame;

    public void UpdateCollectable(EventPayload payload)
    {
        CollectableItem collectable = EventReader.Get<CollectableItem>(payload);
        EventDispatcher.Raise(_checkCollectableRequest, collectable);
        EventDispatcher.Raise(_updateCollectablesStatus);
        EventDispatcher.Raise(_saveGame);
    }

    public void ResetAllCollectables()
    {
        foreach (CollectableItem collectable in _allCollectablesListReference.CollectablesList)
        {
            if (collectable.ItemAmountType == CollectionItemAmount.SingleItem)
            {
                collectable.SetCollectableStatus(false);
                collectable.SetCurrentAmount(0);
            }

            for (int i = 0; i < collectable.MultiCollectables; i++)
            {
                collectable.SetCollectableStatusFromList(i, false);
                collectable.SetCurrentAmountFromList(i, 0);
            }
        }

        EventDispatcher.Raise(_updateCollectablesStatus);
    }

    #region Saving & Loading

    public void UpdateData(EventPayload payload)
    {
        GameData gameData = EventReader.Get<GameData>(payload);
        bool isLoading = EventReader.Get<bool>(payload);

        if (isLoading)
        {
            LoadCollectableStatusFromGameData(gameData);
        }
        else
        {
            SaveCollectableStatusToGameData(gameData);
        }

        EventDispatcher.Raise(_updateProgression, gameData);
    }

    private void LoadCollectableStatusFromGameData(GameData gameData)
    {
        foreach (CollectableItem collectable in _allCollectablesListReference.CollectablesList)
        {
            collectable.LoadCollectableStatus(gameData);
        }

        EventDispatcher.Raise(_updateCollectablesStatus, gameData);
    }

    private void SaveCollectableStatusToGameData(GameData gameData)
    {
        foreach (CollectableItem collectable in _allCollectablesListReference.CollectablesList)
        {
            collectable.SaveCollectableStatus(gameData);
        }
    }

    #endregion
}