using UnityEngine;

[CreateAssetMenu(fileName = "AchievementReferenceHolder", menuName = "Scriptable Objects/Achievements/Achievement Reference Holder")]
public class AchievementReferenceHolderSO : ScriptableObject
{
    [SerializeField] private AchievementInfoSO _achievementToUnlock;
    public string AchievementId
    {
        get
        {
            return _achievementToUnlock.AchievementId;
        }
    }
    public CollectableTypeListSO CollectableTypeList
    {
        get
        {
            return _achievementToUnlock.CollectableList;
        }
    }
}
