using UnityEngine;

public class ShowAnimationDuration : StateMachineBehaviour
{
    [SerializeField] private GameEventEmpty startAchievementPopupCooldown;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        startAchievementPopupCooldown.RaiseEmptyEvent();
    }
}
