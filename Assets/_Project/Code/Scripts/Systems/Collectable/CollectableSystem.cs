using System.Collections.Generic;
using UnityEngine;

public class CollectableSystem : MonoBehaviour
{
    [Header("Collectable references")]
    [SerializeField] private CollectableList _allCollectablesListReference;
    [Header("Event references")]
    [SerializeField] private EventPackage _checkCollectableRequest;
    [SerializeField] private EventPackage _updateCollectablesStatus;
    [SerializeField] private EventPackage _updateProgression;
    [SerializeField] private EventPackage _saveGame;
    public void UpdateCollectable(EventData eventData)
    {
        CollectableAsset collectable = EventPackageExtractor.ExtractEventData<CollectableAsset>(eventData);
        EventPackageFactory.BuildAndInvoke(_checkCollectableRequest, collectable);
        EventPackageFactory.BuildAndInvoke(_updateCollectablesStatus);
        EventPackageFactory.BuildAndInvoke(_saveGame);
    }
    public void ResetAllCollectables()
    {
        foreach (CollectableAsset collectable in _allCollectablesListReference.CollectablesList)
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
        EventPackageFactory.BuildAndInvoke(_updateCollectablesStatus);
    }
    #region Saving & Loading
    public void UpdateData(EventData eventData)
    {
        GameData gameData = EventPackageExtractor.ExtractEventData<GameData>(eventData);
        bool isLoading = EventPackageExtractor.ExtractEventData<bool>(eventData);
        if (isLoading)
        {
            LoadCollectableStatusFromGameData(gameData);
        }
        else
        {
            SaveCollectableStatusToGameData(gameData);
        }
        EventPackageFactory.BuildAndInvoke(_updateProgression, gameData);
    }
    private void LoadCollectableStatusFromGameData(GameData gameData)
    {
        foreach (CollectableAsset collectable in _allCollectablesListReference.CollectablesList)
        {
            collectable.LoadCollectableStatus(gameData);
        }
        EventPackageFactory.BuildAndInvoke(_updateCollectablesStatus, gameData);
    }
    private void SaveCollectableStatusToGameData(GameData gameData)
    {
        List<CollectableAsset>.Enumerator enumAllCollectables = _allCollectablesListReference.CollectablesList.GetEnumerator();
        try
        {
            while (enumAllCollectables.MoveNext())
            {
                CollectableAsset collectable = enumAllCollectables.Current;
                collectable.SaveCollectableStatus(gameData);
            }
        }
        finally
        {
            enumAllCollectables.Dispose();
        }
    }
    #endregion
}
