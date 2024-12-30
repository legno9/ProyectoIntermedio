using Unity.Cinemachine;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AimTarget : MonoBehaviour
{
    [SerializeField] private float maxDistance = 1000f;
    [SerializeField] private LayerMask layerMask = Physics.DefaultRaycastLayers;
    private float lastHitDistance = 0f;

    [SerializeField] private CameraController cameraController;
    [SerializeField] private Transform lookAtTransform;
    [SerializeField] private CinemachineOrbitalFollow notAimingCam;
    [SerializeField] private CinemachineOrbitalFollow aimingCam;
    private Camera mainCamera;

    private void Awake()
    {
        lastHitDistance = maxDistance;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        float clippingDistance;

        if (cameraController.IsAiming)
        {
            clippingDistance = aimingCam.Orbits.Center.Radius + 3;
        }
        else
        {
            clippingDistance = notAimingCam.Orbits.Center.Radius + 3;
        }

        Vector3 minimunAimPoint = mainCamera.transform.position + mainCamera.transform.forward * clippingDistance;

        Vector3 rayOrigin = mainCamera.transform.position;
        Vector3 rayDirection = mainCamera.transform.forward;
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, maxDistance, layerMask))
        {
            if (hit.distance < clippingDistance)
            {
                transform.position = minimunAimPoint;
                lastHitDistance = clippingDistance;
            }
            else
            {
                transform.position = hit.point;
                lastHitDistance = hit.distance;
            }
        }
        else
        {
            transform.position = rayOrigin + mainCamera.transform.forward * lastHitDistance;
        }
    }
}
