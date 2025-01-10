using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move away from target", story: "[Self] moves away from [target]", category: "Action", id: "12501df8a850c23c0f7e426b42b6168f")]
public partial class MoveAwayFromTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1.0f);
    [SerializeReference] public BlackboardVariable<float> DistanceToTarget = new BlackboardVariable<float>(1f);
    [SerializeReference] public BlackboardVariable<bool> Continuous = new BlackboardVariable<bool>(false);
    private NavMeshAgent m_NavMeshAgent;
    private float m_PreviousStoppingDistance;
    private Vector3 locationPosition;

    protected override Status OnStart()
    {
        m_NavMeshAgent = Self.Value.GetComponentInChildren<NavMeshAgent>();

        if (m_NavMeshAgent != null)
        {
            if (m_NavMeshAgent.isOnNavMesh)
            {
                m_NavMeshAgent.ResetPath();
            }

            // Ajusta velocidad y distancia de parada
            m_NavMeshAgent.speed = Speed.Value;
            m_PreviousStoppingDistance = m_NavMeshAgent.stoppingDistance;
            m_NavMeshAgent.stoppingDistance = 0f; // Sin distancia de parada para asegurar la posición final
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // Comprueba si llegó al destino calculado
        if (m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)
        {
            if (Continuous.Value)
            {
                m_NavMeshAgent.ResetPath();
                CalculateNextPosition();
                m_NavMeshAgent.SetDestination(locationPosition);

                return Status.Running;
            }
            else
            {
                return Status.Success;
            }
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (m_NavMeshAgent != null)
        {
            if (m_NavMeshAgent.isOnNavMesh)
            {
                m_NavMeshAgent.ResetPath();
            }

            // Restaura la distancia de parada previa
            m_NavMeshAgent.stoppingDistance = m_PreviousStoppingDistance;
        }
    }

    private void CalculateNextPosition()
    {
        // Calcula la dirección para alejarse del objetivo
        Vector3 directionAwayFromTarget = Self.Value.transform.position - Target.Value.transform.position;
        directionAwayFromTarget.Normalize();

        // Calcula la nueva posición a una distancia suficiente de "DistanceToTarget"
        locationPosition = Target.Value.transform.position + directionAwayFromTarget * DistanceToTarget.Value;
    }
}

