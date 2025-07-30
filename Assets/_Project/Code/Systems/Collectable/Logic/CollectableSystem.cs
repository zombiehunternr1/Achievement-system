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
        CollectableItem collectable = EventPackageExtractor.ExtractEventData<CollectableItem>(eventData);
        ExecuteEventPackage(_checkCollectableRequest, collectable);
        ExecuteEventPackage(_updateCollectablesStatus);
        ExecuteEventPackage(_saveGame);
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
        ExecuteEventPackage(_updateCollectablesStatus);
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
        ExecuteEventPackage(_updateProgression, gameData);
    }
    private void ExecuteEventPackage(EventPackage package, object arg = null)
    {
        EventPackageFactory.BuildAndInvoke(package, arg);
    }
    private void LoadCollectableStatusFromGameData(GameData gameData)
    {
        foreach (CollectableItem collectable in _allCollectablesListReference.CollectablesList)
        {
            collectable.LoadCollectableStatus(gameData);
        }
        ExecuteEventPackage(_updateCollectablesStatus, gameData);
    }
    private void SaveCollectableStatusToGameData(GameData gameData)
    {
        List<CollectableItem>.Enumerator enumAllCollectables = _allCollectablesListReference.CollectablesList.GetEnumerator();
        try
        {
            while (enumAllCollectables.MoveNext())
            {
                enumAllCollectables.Current.SaveCollectableStatus(gameData);
            }
        }
        finally
        {
            enumAllCollectables.Dispose();
        }
    }
    #endregion
}
