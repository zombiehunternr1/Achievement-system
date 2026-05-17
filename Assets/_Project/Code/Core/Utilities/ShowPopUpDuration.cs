using UnityEngine;

public class ShowPopUpDuration : StateMachineBehaviour
{
    [SerializeField] private EventChannel _startAchievementPopupCooldown;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventDispatcher.Raise(_startAchievementPopupCooldown);
    }
}