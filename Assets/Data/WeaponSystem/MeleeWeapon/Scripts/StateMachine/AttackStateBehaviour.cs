using UnityEngine;
public class AttackStateBehaviour : StateMachineBehaviour
{
    private readonly float exitTimePercentage = 0.9f;
    private bool ended = false;
    private float duration;
    private readonly float bufferAttackTime = 1f;
    private bool canBuffer;
    private PlayerMeleeAnimation playerAnimation;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerAnimation == null) 
        {
            playerAnimation = animator.GetComponentInParent<PlayerMeleeAnimation>();
        }
        duration = stateInfo.length;
        ended = false;
        canBuffer = duration < bufferAttackTime * 2;
        if (canBuffer){playerAnimation.OnCanBuffer.Invoke();}
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (stateInfo.normalizedTime * duration >= duration - bufferAttackTime && !canBuffer)
        {
            canBuffer = true;
            playerAnimation.OnCanBuffer.Invoke();
        }

        if (stateInfo.normalizedTime >= exitTimePercentage && !ended)
        {
            ended = true;
            animator.SetBool("IsAttacking", false);
            playerAnimation.OnAttackAnimationComplete.Invoke();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("CanAttack", true);
    }
}
