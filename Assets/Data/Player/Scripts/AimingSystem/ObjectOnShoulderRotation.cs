using Unity.Cinemachine;
using UnityEngine;

public class ObjectOnShoulderRotation : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow cam1;
    [SerializeField] private CinemachineOrbitalFollow cam2;

    private void Update()
    {
        if (cam1.gameObject.activeSelf)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, cam1.HorizontalAxis.Value, transform.eulerAngles.z);
        }
        else
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, cam2.HorizontalAxis.Value, transform.eulerAngles.z);
        }
    }
}
