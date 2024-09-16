using System.Collections.Generic;
using UnityEngine;

public class CollectableSystem : MonoBehaviour
{
    [SerializeField] private GenericEmptyEvent _saveGameEvent;
    [SerializeField] private GenericEmptyEvent _updateCollectablesEvent;
    [SerializeField] private UpdateProgressionEvent _updateProgressionEvent;
    [SerializeField] private UpdateAchievementsEvent _updateAchievementsEvent;
    [SerializeField] private CollectableListHolder _allcollectableListsReference;
    public void UpdateCollectableStatus()
    {
        int collecteditems = 0;
        foreach (CollectableTypeListSO collectableTypeList in _allcollectableListsReference.AllCollectableLists)
        {
            foreach(CollectableTypeSO collectableType in collectableTypeList.CollectablesList)
            {
                if (collectableType.IsCollected)
                {
                    collecteditems++;
                    foreach(AchievementReferenceHolderSO achievementEvent in _updateAchievementsEvent.achievementReferences)
                    {
                        if(achievementEvent.CollectableTypeList != null && achievementEvent.CollectableTypeList.CollectablesList.Contains(collectableType))
                        {
                            _updateAchievementsEvent.Invoke(achievementEvent.AchievementId, collecteditems, null);
                        }
                    }
                }
            }
        }
        _saveGameEvent.Invoke();
    }
    public void ResetAllCollectibles()
    {
        foreach (CollectableTypeListSO collectableTypeList in _allcollectableListsReference.AllCollectableLists)
        {
            foreach(CollectableTypeSO collectable in collectableTypeList.CollectablesList)
            {
                collectable.SetCollectableStatus(false);
            }
        }
        _updateCollectablesEvent.Invoke();
    }
    public void UpdateData(GameData data, bool isLoading)
    {
        if (isLoading)
        {
            foreach (CollectableTypeListSO collectableTypeList in _allcollectableListsReference.AllCollectableLists)
            {
                foreach(CollectableTypeSO collectableType in collectableTypeList.CollectablesList)
                {
                    data.TotalCollectionsData.TryGetValue(collectableType.CollectableId, out bool isCollected);
                    collectableType.SetCollectableStatus(isCollected);
                }
            }
            _updateCollectablesEvent.Invoke();
        }
        else
        {
            List<CollectableTypeListSO>.Enumerator enumAllCollectablesLists = _allcollectableListsReference.AllCollectableLists.GetEnumerator();
            try
            {
                while (enumAllCollectablesLists.MoveNext())
                {
                    List<BaseCollectableTypeSO>.Enumerator enumCurrentCollectableList = enumAllCollectablesLists.Current.CollectablesList.GetEnumerator();
                    while (enumCurrentCollectableList.MoveNext())
                    {
                        string id = enumCurrentCollectableList.Current.CollectableId;
                        bool value = enumCurrentCollectableList.Current.IsCollected;
                        if (data.TotalCollectionsData.ContainsKey(id))
                        {
                            data.TotalCollectionsData.Remove(id);
                        }
                        data.TotalCollectionsData.Add(id, value);
                    }
                }
            }
            finally
            {
                enumAllCollectablesLists.Dispose();
            }
        }
        _updateProgressionEvent.Invoke(data);
    }
}
