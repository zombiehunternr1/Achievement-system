using UnityEngine;

public class ShowAnimationDuration : StateMachineBehaviour
{
    [SerializeField] private GenericEmptyEvent _startAchievementPopupCooldown;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _startAchievementPopupCooldown.Invoke();
    }
}
