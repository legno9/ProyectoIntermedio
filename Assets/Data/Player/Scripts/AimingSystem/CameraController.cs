using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(PlayerInput))]
public class CameraController : MonoBehaviour
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
    [SerializeField] private LayerMask lockOnBlockingLayers;
    private Queue<Transform> lockOnTargets = new();
    private bool isLockedOn = false;

    [Header("Look At")]
    [SerializeField] private Transform lookAtTransform;
    [SerializeField] private Transform notAimingLookAtTransform;
    [SerializeField] private Transform aimingLookAtTransform;
    private Transform currentLockOnTransform;
    private Transform newLockOnTransform;
    private bool isSwitchingLockOnTarget = false;
    [SerializeField] private float transitionTime = 0.25f;
    private float elapsedTime = 0f;
    private Vector3 startPosition;

    [Header("Rigs")]
    [SerializeField] private Rig aimForwardRig;
    [SerializeField] private Transform objectInFrontHolder;

    private void Awake()
    {
        cam2.gameObject.SetActive(false);
        currentOrbitalFollow = cam1;
        currentCam = cam1.GetComponent<CinemachineCamera>();
        mainCamera = Camera.main;
        //cam1.GetComponent<CinemachineCamera>().LookAt = lookAtTransform;
        cam2.GetComponent<CinemachineCamera>().LookAt = lookAtTransform;

        lookAtTransform.position = notAimingLookAtTransform.position;
        currentLockOnTransform = notAimingLookAtTransform;
    }

    private void LateUpdate()
    {
        if (!isLockedOn && IsAiming)
        {
            LockOnToNextTarget();
        }

        if (isLockedOn)
        {
            transitionTime = 0.25f;
        }
        else
        {
            transitionTime = 0.001f;
        }

        if (IsAiming)
        {
            Vector3 targetdirection = Vector3.ProjectOnPlane(lookAtTransform.position - mainCamera.transform.position, Vector3.up);
            float angletotarget = Vector3.SignedAngle(Vector3.forward, targetdirection, Vector3.up);
            currentOrbitalFollow.HorizontalAxis.Value = angletotarget;
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

        if (playerWeaponManager.GetCurrentWeaponIsRanged())
        {
            aimForwardRig.weight = 1;
            if (isLockedOn)
            {
                objectInFrontHolder.rotation = Quaternion.LookRotation(lookAtTransform.position - objectInFrontHolder.position);
            }
            else
            {
                float value = currentOrbitalFollow.VerticalAxis.Value;

                objectInFrontHolder.localEulerAngles = new Vector3(
                    Mathf.Lerp(-37, 17, Mathf.Clamp01((value - -10) / (45 - -10))),
                    objectInFrontHolder.localRotation.y,
                    objectInFrontHolder.localRotation.z
                );
            }
        }
        else
        {
            aimForwardRig.weight = 0;
            objectInFrontHolder.rotation = Quaternion.identity;
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
        //cam1.gameObject.SetActive(false);
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

        cam1.gameObject.SetActive(true);
        cam2.gameObject.SetActive(false);
        cam1.HorizontalAxis.Value = cam2.HorizontalAxis.Value;
        cam1.VerticalAxis.Value = cam2.VerticalAxis.Value;
        currentOrbitalFollow = cam1;
        currentCam = cam1.GetComponent<CinemachineCamera>();
        DisableLockOn();

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
                while (newTarget == currentLockOnTransform)
                {
                    if (lockOnTargets.TryDequeue(out newTarget))
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }

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
        if (currentOrbitalFollow == cam2)
        {
            newLockOnTransform = aimingLookAtTransform;
        }
        else
        {
            newLockOnTransform = notAimingLookAtTransform;
        }

        startPosition = lookAtTransform.position;
        elapsedTime = 0f;

        isSwitchingLockOnTarget = true;
        isLockedOn = false;
    }

    private void SearchTargets()
    {
        List<Collider> targets = Physics.OverlapSphere(playerTransform.position, lockOnRange, lockOnLayer).ToList();
        List<Collider> collidersIgnored = new List<Collider>();

        foreach (var target in targets)
        {
            if (Physics.Linecast(mainCamera.transform.position, target.transform.position, out RaycastHit hit, lockOnBlockingLayers) ||
                (hit.collider && target))
            {
                collidersIgnored.Add(target);
            }
        }

        foreach (var target in collidersIgnored)
        {
            targets.Remove(target);
        }

        // Filter, sort, and queue transforms based on distance
        lockOnTargets = new Queue<Transform>(
            targets
            .Select(hit => hit.transform) // Get transforms
            .OrderBy(transform => Vector3.Distance(playerTransform.position, transform.position)) // Sort by proximity
        );
    }
}
