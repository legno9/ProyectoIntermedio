using UnityEngine;

public class BaseStateBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (layerIndex == 1) // Ensure this is the attack layer
        {
            animator.GetComponent<BossController>().OnAttackFinished();
        }
    }
}
