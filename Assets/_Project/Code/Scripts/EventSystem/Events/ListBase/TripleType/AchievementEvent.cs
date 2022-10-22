using UnityEngine;

[CreateAssetMenu(fileName = "AchievementEvent", menuName = "Scriptable Objects/New Events/Achievement")]
public class AchievementEvent : BaseEventListTripleGenericType<int, int?, float?>
{
    [SerializeField] private AchievementInfoSO _AchievementToUnlock;
    public int AchievementID
    {
        get
        {
            return _AchievementToUnlock.AchievementId;
        }
    }
}
