using UnityEngine;

[CreateAssetMenu(fileName = "AchievementEvent", menuName = "Scriptable Objects/Events/Type/Achievement")]
public class AchievementEvent : BaseEventListTripleGenericType<string, int?, float?>
{
    [SerializeField] private AchievementInfoSO _AchievementToUnlock;
    public string AchievementID
    {
        get
        {
            return _AchievementToUnlock.achievementId;
        }
    }
}
