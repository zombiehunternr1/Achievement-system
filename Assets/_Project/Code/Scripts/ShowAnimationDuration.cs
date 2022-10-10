using UnityEngine;

public class ShowAnimationDuration : StateMachineBehaviour
{
    [SerializeField] private GameEventEmpty _startAchievementPopupCooldown;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _startAchievementPopupCooldown.RaiseEmptyEvent();
    }
}
