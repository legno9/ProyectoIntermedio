using Unity.Cinemachine;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ObjectToAimAt : MonoBehaviour
{
    private CinemachineOrbitalFollow orbitalFollow;
    [SerializeField] private float maxDistance = 1000f;
    [SerializeField] private LayerMask layerMask = Physics.DefaultRaycastLayers;
    private Camera mainCamera;

    private void Awake()
    {
        orbitalFollow = GetComponentInParent<CinemachineOrbitalFollow>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        float lerpValue = Mathf.Clamp01((orbitalFollow.VerticalAxis.Value - -10) / (20 - -10));
        float clippingDistance = Mathf.Lerp(orbitalFollow.Orbits.Bottom.Radius, orbitalFollow.Orbits.Center.Radius, lerpValue);
        Vector3 rayOrigin = mainCamera.transform.position + mainCamera.transform.forward * (clippingDistance + 1);
        Vector3 rayDirection = mainCamera.transform.forward;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, maxDistance, layerMask))
        {
            transform.position = hit.point;
        }
        else
        {
            transform.position = rayOrigin + mainCamera.transform.forward * maxDistance;
        }
    }
}
