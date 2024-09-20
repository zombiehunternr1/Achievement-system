using UnityEngine;

public class ShowPopUpDuration : StateMachineBehaviour
{
    [SerializeField] private EmptyEvent _startAchievementPopupCooldown;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _startAchievementPopupCooldown.Invoke();
    }
}
