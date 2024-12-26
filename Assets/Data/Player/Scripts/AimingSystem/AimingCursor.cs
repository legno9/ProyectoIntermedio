using UnityEngine;
using UnityEngine.InputSystem;

public class AimingCursor : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask = Physics.DefaultRaycastLayers;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            transform.position = hit.point;
        }
    }
}
