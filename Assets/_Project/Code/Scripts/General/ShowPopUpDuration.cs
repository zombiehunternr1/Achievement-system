using UnityEngine;

public class ShowPopUpDuration : StateMachineBehaviour
{
    [SerializeField] private EventPackage _startAchievementPopupCooldown;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventPackageFactory.BuildAndInvoke(_startAchievementPopupCooldown);
    }
}
