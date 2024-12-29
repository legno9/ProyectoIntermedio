using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static System.TimeZoneInfo;

[RequireComponent(typeof(PlayerInput))]
public class NewCameraController : MonoBehaviour
{
    [Header("Aim")]
    [SerializeField] private CinemachineOrbitalFollow cam1;
    [SerializeField] private CinemachineOrbitalFollow cam2;
    private CinemachineOrbitalFollow currentOrbitalFollow;
    private CinemachineCamera currentCam;
    private Camera mainCamera;
    [SerializeField] private EntityWeaponManager playerWeaponManager;
    private bool isAiming = false;
    public bool IsAiming => isAiming;

    [Header("Lock-On")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float lockOnRange;
    [SerializeField] private LayerMask lockOnLayer;
    private Queue<Transform> lockOnTargets = new();
    private bool isLockedOn = false;

    [Header("Look At")]
    [SerializeField] private Transform lookAtTransform;
    [SerializeField] private Transform notAimingLookAtTransform;
    private Transform currentLockOnTransform;
    private Transform newLockOnTransform;
    private bool isSwitchingLockOnTarget = false;
    [SerializeField] private float transitionTime = 0.25f;
    private float elapsedTime = 0f;
    private Vector3 startPosition;

    private void Awake()
    {
        cam2.gameObject.SetActive(false);
        currentOrbitalFollow = cam1;
        currentCam = cam1.GetComponent<CinemachineCamera>();
        mainCamera = Camera.main;

        lookAtTransform.position = notAimingLookAtTransform.position;
        currentLockOnTransform = notAimingLookAtTransform;
    }

    private void LateUpdate()
    {
        if (isLockedOn)
        {
            Vector3 targetDirection = Vector3.ProjectOnPlane(lookAtTransform.position - mainCamera.transform.position, Vector3.up);
            float angleToTarget = Vector3.SignedAngle(Vector3.forward, targetDirection, Vector3.up);
            currentOrbitalFollow.HorizontalAxis.Value = angleToTarget;
        }

        if (isSwitchingLockOnTarget)
        {
            // Update elapsed time
            elapsedTime += Time.deltaTime;

            // Calculate the interpolation factor
            float t = Mathf.Clamp01(elapsedTime / transitionTime);

            // Smoothly interpolate position
            lookAtTransform.position = Vector3.Lerp(startPosition, newLockOnTransform.position, t);

            // Check if transition is complete
            if (t >= 1f)
            {
                currentLockOnTransform = newLockOnTransform; // Set the new target as the current target
                isSwitchingLockOnTarget = false;
            }
        }
        else if (currentLockOnTransform != null)
        {
            // Follow the current target's position
            lookAtTransform.position = currentLockOnTransform.position;
        }
    }

    private void OnAim(InputValue value)
    {
        if (playerWeaponManager.GetCurrentWeaponIsRanged())
        {
            if (value.Get<float>() == 1)
            {
                EnableAiming();
            }
            else
            {
                DisableAiming();
            }
        }
    }

    private void EnableAiming()
    {
        cam1.gameObject.SetActive(false);
        cam2.gameObject.SetActive(true);
        cam2.HorizontalAxis.Value = cam1.HorizontalAxis.Value;
        cam2.VerticalAxis.Value = cam1.VerticalAxis.Value;
        currentOrbitalFollow = cam2;
        currentCam = cam2.GetComponent<CinemachineCamera>();

        SearchTargets();
        LockOnToNextTarget();

        isAiming = true;
    }

    private void DisableAiming()
    {
        DisableLockOn();

        cam1.gameObject.SetActive(true);
        cam2.gameObject.SetActive(false);
        cam1.HorizontalAxis.Value = cam2.HorizontalAxis.Value;
        cam1.VerticalAxis.Value = cam2.VerticalAxis.Value;
        currentOrbitalFollow = cam1;
        currentCam = cam1.GetComponent<CinemachineCamera>();

        isAiming = false;
    }

    private void OnNextTarget(InputValue value)
    {
        if (isAiming)
        {
            LockOnToNextTarget();
        }
    }

    private void LockOnToNextTarget()
    {
        if (lockOnTargets.TryDequeue(out var target))
        {
            LockOnToTarget(target);
        }
        else
        {
            SearchTargets();

            if (lockOnTargets.TryDequeue(out var newTarget))
            {
                LockOnToTarget(newTarget);
            }
            else
            {
                DisableLockOn();
            }
        }
    }

    private void LockOnToTarget(Transform target)
    {
        newLockOnTransform = target;
        startPosition = lookAtTransform.position;
        elapsedTime = 0f;

        isSwitchingLockOnTarget = true;
        isLockedOn = true;
    }

    private void DisableLockOn()
    {
        newLockOnTransform = notAimingLookAtTransform;
        startPosition = lookAtTransform.position;
        elapsedTime = 0f;

        isSwitchingLockOnTarget = true;
        isLockedOn = false;
    }

    private void SearchTargets()
    {
        Collider[] hits = Physics.OverlapSphere(playerTransform.position, lockOnRange, lockOnLayer);

        // Filter, sort, and queue transforms based on distance
        lockOnTargets = new Queue<Transform>(
            hits
            .Select(hit => hit.transform) // Get transforms
            .OrderBy(transform => Vector3.Distance(playerTransform.position, transform.position)) // Sort by proximity
        );
    }
}
