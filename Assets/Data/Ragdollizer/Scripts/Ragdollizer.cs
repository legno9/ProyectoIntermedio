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

    private void UnRagdollize()
    {
        foreach (Collider collider in colliders) { collider.enabled = false; }
        foreach (Rigidbody rigidbody in rigidbodies) { rigidbody.isKinematic = true; }
    }

    public void Ragdollize()
    {
        foreach (Collider collider in colliders) { collider.enabled = true; }
        foreach (Rigidbody rigidbody in rigidbodies) { rigidbody.isKinematic = false; }
    }
}
