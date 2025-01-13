using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour, IMovingAnimatable
{
    public static PlayerMovement Instance { get; private set; }

    [Header("Movement")]
    [SerializeField] private float linearAcceleration = 50f;
    [SerializeField] private float maxSpeed = 5.0f;
    [SerializeField] private float decelerationFactor = 10.0f;
    [SerializeField] private float timePerStep = 0.3f;
    private float lastStepTime;
    [SerializeField] private AudioClipList stepSounds;

    public enum OrientationMode
    {
        MovementDirection,
        CameraDirection,
        FaceToTarget
    }

    [Header("Orientation")]
    [SerializeField] private OrientationMode orientationMode = OrientationMode.MovementDirection;
    [SerializeField] private float angularVelocity = 360f;
    [SerializeField] private Transform target;

    private CharacterController characterController;
    private Camera mainCamera;
    [HideInInspector] public bool canMove = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        lastStepTime = Time.time;
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    private Vector3 rawStickValue = Vector3.zero;
    private void Update()
    {
        Vector3 compositeMovement = Vector3.zero;
        if (canMove)
        {
            compositeMovement += UpdateMovementOnPlane();
        }
        compositeMovement += UpdateVerticalMovement();

        characterController.Move(compositeMovement);

        UpdateOrientation();
    }

    Vector3 velocityOnPlane = Vector3.zero;
    private Vector3 UpdateMovementOnPlane()
    {
        //Frenado
        Vector3 decelerationOnPlane = -velocityOnPlane * decelerationFactor * Time.deltaTime;
        velocityOnPlane += decelerationOnPlane;

        //Aceleraci�n y velocidad
        Vector3 acceleration = (mainCamera.transform.forward * rawStickValue.z) + (mainCamera.transform.right * rawStickValue.x);
        Vector3 projectedAcceleration = Vector3.ProjectOnPlane(acceleration, Vector3.up).normalized;
        Vector3 deltaAcceleration = projectedAcceleration * linearAcceleration * Time.deltaTime;

        Vector3 currentSpeed = velocityOnPlane;
        velocityOnPlane += deltaAcceleration;

        if (currentSpeed.magnitude <= maxSpeed)
        {
            velocityOnPlane = Vector3.ClampMagnitude(velocityOnPlane, maxSpeed);
        }
        else
        {
            float shortestAngle = Vector3.Angle(currentSpeed, velocityOnPlane);
            float lerpValue = Mathf.Clamp(shortestAngle, 0, 90) / 90;
            float lerpedMaxSpeed = Mathf.Lerp(maxSpeed, currentSpeed.magnitude, lerpValue);
            velocityOnPlane = Vector3.ClampMagnitude(velocityOnPlane, lerpedMaxSpeed);
        }

        Vector3 movement = velocityOnPlane * Time.deltaTime;

        if (rawStickValue.magnitude > 0.1f && Time.time - lastStepTime > timePerStep)
        {
            lastStepTime = Time.time;
            stepSounds.PlayAtPointRandom(transform.position);
        }

        return movement;
    }

    float verticalVelocity = 0;
    const float GRAVITY = -9.8f;
    private Vector3 UpdateVerticalMovement()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = 0f;
        }

        verticalVelocity += GRAVITY * Time.deltaTime;
        return verticalVelocity * Vector3.up * Time.deltaTime;
    }

    Vector3 lastMovementDirection = Vector3.zero;
    private void UpdateOrientation()
    {
        Vector3 desiredDirection = CalculateDesiredDirection();

        RotateToDesiredDirection(desiredDirection);
    }

    private Vector3 CalculateDesiredDirection()
    {
        Vector3 desiredDirection = Vector3.zero;

        switch (orientationMode)
        {
            case OrientationMode.MovementDirection:
                if (rawStickValue.magnitude < 0.01f)
                {
                    desiredDirection = lastMovementDirection;
                }
                else
                {
                    desiredDirection = velocityOnPlane;
                    lastMovementDirection = desiredDirection;
                }
                break;
            case OrientationMode.CameraDirection:
                desiredDirection = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
                break;
            case OrientationMode.FaceToTarget:
                if (target != null)
                {
                    desiredDirection = Vector3.ProjectOnPlane(target.position - transform.position, Vector3.up);
                }
                else
                {
                    desiredDirection = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
                }
                break;
        }

        return desiredDirection;
    }

    private void RotateToDesiredDirection(Vector3 desiredDirection)
    {
        // Codigo que gira para coincidir con direcci�n
        float angularDistance = Vector3.SignedAngle(transform.forward, desiredDirection, Vector3.up);

        if (Mathf.Abs(angularDistance) < 0.01f) return;

        if (orientationMode == OrientationMode.FaceToTarget)
        {
            transform.rotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
        }
        else
        {
            float angleToApply = angularVelocity * Time.deltaTime;
            angleToApply = Mathf.Min(angleToApply, Mathf.Abs(angularDistance));

            Quaternion rotationToApply =
                Quaternion.AngleAxis(
                    angleToApply * Mathf.Sign(angularDistance),
                    Vector3.up);
            transform.rotation *= rotationToApply;
        }
    }

    #region Input Events
    private void OnMove(InputValue value)
    {
        Vector2 stickValue = value.Get<Vector2>();

        rawStickValue = Vector3.forward * stickValue.y + Vector3.right * stickValue.x;
    }
    #endregion


    public Transform GetTransform()
    {
        return transform;
    }

    public float GetNormalizedForwardVelocity()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(velocityOnPlane);
        return localVelocity.z / maxSpeed;
    }

    public float GetNormalizedHorizontalVelocity()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(velocityOnPlane);
        return localVelocity.x / maxSpeed;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
