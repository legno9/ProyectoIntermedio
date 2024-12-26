using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(-1)]
[RequireComponent(typeof(NavMeshAgent))]
public class Entity : MonoBehaviour
{
    internal NavMeshAgent agent;
    //internal EntitySight sight;
    internal EntityWeaponManager weaponManager;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //sight = GetComponentInChildren<EntitySight>();
        weaponManager = GetComponent<EntityWeaponManager>();
    }
}