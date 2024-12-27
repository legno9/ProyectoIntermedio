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
    private bool lockedOn = false;

    [SerializeField] Transform lookAtTransform;
    private Transform currentLockOnTransform;
    private Transform newLockOnTransform;
    private bool isSwitchingLockOnTarget = false;
    [SerializeField] private float transitionTime = 0.25f; // Time to complete the transition in seconds
    private float elapsedTime = 0f; // Tracks elapsed time for the transition
    private Vector3 startPosition; // Start position for the transition

    private void Awake()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
        orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
    }

    private void Update()
    {
        if (lockedOn)
        {
            Vector3 targetDirection = Vector3.ProjectOnPlane(cinemachineCamera.LookAt.position - transform.position, Vector3.up);
            float angleToTarget = Vector3.Angle(Vector3.forward, targetDirection);
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
        if (value.Get<float>() == 1)
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
        }
        else
        {
            DisableLockOn();

            aimTransitionTween.Kill();
            aimTransitionTween = DOTween.To(
                () => orbitalFollow.VerticalAxis.Value,
                (x) => orbitalFollow.VerticalAxis.Value = x,
                20f,
                0.3f
            );
        }
    }

    private void OnNextTarget(InputValue value)
    {
        LockOnToNextTarget();
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
        if (target != currentLockOnTransform)
        {
            newLockOnTransform = target;
            startPosition = lookAtTransform.position;
            elapsedTime = 0f;
            isSwitchingLockOnTarget = true;
        }

        cinemachineCamera.LookAt = lookAtTransform;
        cameraLook.action.Disable();
        lockedOn = true;
    }

    private void DisableLockOn()
    {
        cinemachineCamera.LookAt = objectOnShoulderTransform;
        cameraLook.action.Enable();
        lockedOn = false;
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
