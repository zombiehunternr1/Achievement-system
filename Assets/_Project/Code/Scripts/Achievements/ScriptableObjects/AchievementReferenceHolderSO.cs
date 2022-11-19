using UnityEngine;

[CreateAssetMenu(fileName = "AchievementReferenceHolder", menuName = "Scriptable Objects/Achievements/Achievement Reference Holder")]
public class AchievementReferenceHolderSO : ScriptableObject
{
    [SerializeField] private AchievementInfoSO _achievementToUnlock;
    public string achievementId
    {
        get
        {
            return _achievementToUnlock.achievementId;
        }
    }
}
