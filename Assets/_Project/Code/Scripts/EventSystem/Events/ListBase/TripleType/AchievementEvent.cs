using UnityEngine;

[CreateAssetMenu(fileName = "AchievementEvent", menuName = "Scriptable Objects/Events/Type/Achievement")]
public class AchievementEvent : BaseEventListTripleGenericType<string, int?, float?>
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
