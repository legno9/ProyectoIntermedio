using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class TargetFollower : MonoBehaviour
{
    [SerializeField] float distance = 3;
    [SerializeField] private Transform origin;
    private Transform target;
    private Behaviour[] constraints;

    public void SetTarget(Transform target)
    {
        this.target = target;
        
        bool value =  target? true : false;
        EnableConstraints(value);
    }

    public void SetConstraints(Behaviour[] constraints)
    {
        this.constraints = constraints;
        EnableConstraints(false);
    }

    private void EnableConstraints(bool value)
    {
        foreach (var constraint in constraints)
        {
            if (constraint is Rig rig)
            {
                rig.weight = value ? 1 : 0;
            }
            else
            {
                constraint.enabled = value;
            }
        }
    }

    public void UpdateFollower() 
    {
        if (target != null)
        {
            Vector3 direction = (target.position - origin.position).normalized;
            Vector3 endPosition = origin.position + direction * distance;
            transform.position = endPosition;
        }
    }
}
