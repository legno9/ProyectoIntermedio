using Unity.Cinemachine;
using UnityEngine;

public class ObjectOnShoulderRotation : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow cinemachineOrbitalFollow;

    private void Update()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, cinemachineOrbitalFollow.HorizontalAxis.Value, transform.eulerAngles.z);
    }
}
