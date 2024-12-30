using Unity.Cinemachine;
using UnityEngine;

public class ObjectOnShoulderRotation : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow cam1;
    [SerializeField] private CinemachineOrbitalFollow cam2;
    [SerializeField] private Transform playerTransform;
    private Vector3 offset;

    private void Awake()
    {
        offset = transform.position - playerTransform.position;
    }

    private void Update()
    {
        transform.position = playerTransform.position + offset;

        if (cam1.gameObject.activeSelf)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, cam1.HorizontalAxis.Value, transform.eulerAngles.z);
        }
        else
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, cam2.HorizontalAxis.Value, transform.eulerAngles.z);
        }
    }
}
