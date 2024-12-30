using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ShootAroundTargetAction", story: "[Enemy] shoots around [target]", category: "Action", id: "ab8ae2b981d288089acc54c1abd0bcf3")]
public partial class ShootAroundTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<EnemyController> Enemy;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> OrbitRadius = new BlackboardVariable<float>(5.0f); // Radio del círculo
    [SerializeReference] public BlackboardVariable<float> OrbitSpeed = new BlackboardVariable<float>(50.0f); // Velocidad angular (grados por segundo)


    private NavMeshAgent m_NavMeshAgent;
    private float m_CurrentAngle = 0f; // Ángulo actual en el círculo (en grados)


    protected override Status OnStart()
    {
        if (Enemy.Value == null || Target.Value == null)
        {
            LogFailure("Enemy or Target is null.");
            return Status.Failure;
        }

        m_NavMeshAgent = Enemy.Value.GetComponentInChildren<NavMeshAgent>();
        if (m_NavMeshAgent == null || !m_NavMeshAgent.isOnNavMesh)
        {
            LogFailure("NavMeshAgent is missing or not on the NavMesh.");
            return Status.Failure;
        }

        return Status.Running;
    }
    protected override Status OnUpdate()
    {
        if (Enemy.Value == null || Target.Value == null || m_NavMeshAgent == null)
        {
            return Status.Failure;
        }

        // Disparar hacia el objetivo
        Enemy.Value.Shoot(Target.Value.transform);

        // Calcular el siguiente punto en el círculo
        Vector3 targetPosition = Target.Value.transform.position;
        m_CurrentAngle += OrbitSpeed * Time.deltaTime; // Incrementar el ángulo basado en la velocidad
        m_CurrentAngle %= 360f; // Mantener el ángulo en el rango [0, 360]

        // Convertir el ángulo a radianes y calcular la posición alrededor del objetivo
        float angleInRadians = m_CurrentAngle * Mathf.Deg2Rad;
        Vector3 orbitPosition = new Vector3(
            targetPosition.x + Mathf.Cos(angleInRadians) * OrbitRadius,
            targetPosition.y,
            targetPosition.z + Mathf.Sin(angleInRadians) * OrbitRadius
        );

        // Establecer el destino en el NavMeshAgent
        if (m_NavMeshAgent.isOnNavMesh)
        {
            m_NavMeshAgent.SetDestination(orbitPosition);
        }

        return Status.Running;
    }


    protected override void OnEnd()
    {
        if (m_NavMeshAgent != null && m_NavMeshAgent.isOnNavMesh)
        {
            m_NavMeshAgent.ResetPath();
        }
    }


}

