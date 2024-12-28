using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

[DefaultExecutionOrder(-1)]
[RequireComponent(typeof(PlayerInput))]
public class CameraController : MonoBehaviour
{
    private CinemachineCamera cinemachineCamera;
    private CinemachineOrbitalFollow orbitalFollow;
    private Tween aimTransitionTween;

    private Queue<Transform> lockOnTargets = new();
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform objectOnShoulderTransform;
    [SerializeField] LayerMask lockOnLayer;
    [SerializeField] float lockOnRange;
    [SerializeField] InputActionReference cameraLook;
    private bool isAiming = false;
    private bool isLockedOn = false;

    [SerializeField] Transform lookAtTransform;
    [SerializeField] Transform notAimingLookAtTransform;
    private Transform currentLockOnTransform;
    private Transform newLockOnTransform;
    private bool isSwitchingLockOnTarget = false;
    [SerializeField] private float transitionTime = 0.25f; // Time to complete the transition in seconds
    private float elapsedTime = 0f; // Tracks elapsed time for the transition
    private Vector3 startPosition; // Start position for the transition

    [SerializeField] private EntityWeaponManager playerWeaponManager;

    private void Awake()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
        orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
        lookAtTransform.position = notAimingLookAtTransform.position;
    }

    private void Update()
    {
        if (isAiming && !playerWeaponManager.GetCurrentWeaponIsRanged())
        {
            DisableAiming();
        }

        if (isLockedOn)
        {
            Vector3 targetDirection = Vector3.ProjectOnPlane(cinemachineCamera.LookAt.position - transform.position, Vector3.up);
            float angleToTarget = Vector3.SignedAngle(Vector3.forward, targetDirection, Vector3.up);
            orbitalFollow.HorizontalAxis.Value = angleToTarget;
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
        SearchTargets();
        LockOnToNextTarget();

        aimTransitionTween.Kill();
        aimTransitionTween = DOTween.To(
            () => orbitalFollow.VerticalAxis.Value,
            (x) => orbitalFollow.VerticalAxis.Value = x,
            -10f,
            0.3f
        );

        isAiming = true;
    }

    private void DisableAiming()
    {
        DisableLockOn();

        aimTransitionTween.Kill();
        aimTransitionTween = DOTween.To(
            () => orbitalFollow.VerticalAxis.Value,
            (x) => orbitalFollow.VerticalAxis.Value = x,
            20f,
            0.3f
        );

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

        cinemachineCamera.LookAt = lookAtTransform;
        cameraLook.action.Disable();
        isLockedOn = true;
    }

    private void DisableLockOn()
    {
        newLockOnTransform = notAimingLookAtTransform;
        startPosition = lookAtTransform.position;
        elapsedTime = 0f;
        isSwitchingLockOnTarget = true;

        cinemachineCamera.LookAt = objectOnShoulderTransform;
        cameraLook.action.Enable();
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
