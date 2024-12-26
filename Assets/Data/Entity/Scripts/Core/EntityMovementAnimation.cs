using UnityEngine;

public class EntityMovementAnimation : MonoBehaviour
{
    private Animator animator;
    IMovingAnimatable animatable;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        animatable = GetComponent<IMovingAnimatable>();
    }

    private void Update()
    {
        animator.SetFloat("HorizontalVelocity", animatable.GetNormalizedHorizontalVelocity());
        animator.SetFloat("ForwardVelocity", animatable.GetNormalizedForwardVelocity());
    }
}
