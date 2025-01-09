using UnityEngine;

public class LookToCamera : MonoBehaviour
{
    Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }
}
