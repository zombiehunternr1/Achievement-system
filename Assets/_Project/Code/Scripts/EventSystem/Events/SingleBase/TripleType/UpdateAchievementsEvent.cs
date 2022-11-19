using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="UpdateAchievementsEvent", menuName ="Scriptable Objects/Events/Type/Update Achievements Event")]
public class UpdateAchievementsEvent : BaseEventSingleGenericTripleType<string, int?, float?>
{
    [SerializeField] private List<AchievementReferenceHolderSO> _achievementReferenceHolders;
    public List<AchievementReferenceHolderSO> achievementReferences
    {
        get
        {
            return _achievementReferenceHolders;
        }
    }
}
