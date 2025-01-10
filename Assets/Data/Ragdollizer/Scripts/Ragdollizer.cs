using UnityEngine;

public class Ragdollizer : MonoBehaviour
{
    [SerializeField] private bool debugRagdollize;

    Collider[] colliders;
    Rigidbody[] rigidbodies;

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();

        UnRagdollize();
    }

    [ContextMenu("Unragdollize")]
    private void UnRagdollize()
    {
        foreach (Collider collider in colliders) { collider.enabled = false; collider.gameObject.layer = LayerMask.NameToLayer("Default"); }
        foreach (Rigidbody rigidbody in rigidbodies) { rigidbody.isKinematic = true; }
    }

    [ContextMenu("Ragdollize")]
    public void Ragdollize()
    {
        foreach (Collider collider in colliders) { collider.enabled = true; collider.gameObject.layer = LayerMask.NameToLayer("EnemyDeath");}
        foreach (Rigidbody rigidbody in rigidbodies) { rigidbody.isKinematic = false; }
    }
}
