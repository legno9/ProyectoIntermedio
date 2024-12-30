using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using DG.Tweening;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UniquePatrol", story: "[Self] patrols to first object on [list]", category: "Action", id: "61c2c410c7a66b055d2b2033db88d1e2")]
public partial class UniquePatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<List<GameObject>> List;
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);
    [Tooltip("Should patrol restart from the latest point?")]

    private NavMeshAgent m_NavMeshAgent;
    private float m_PreviousStoppingDistance;

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            LogFailure("No Self assigned.");
            return Status.Failure;
        }

        if (List.Value == null || List.Value.Count == 0)
        {
            LogFailure("No List to patrol assigned.");
            return Status.Failure;
        }

        m_NavMeshAgent = Self.Value.GetComponentInChildren<NavMeshAgent>();
        if (m_NavMeshAgent != null)
        {
            if (m_NavMeshAgent.isOnNavMesh)
            {
                m_NavMeshAgent.ResetPath();
            }
            m_NavMeshAgent.speed = Speed.Value;
            m_NavMeshAgent.stoppingDistance = DistanceThreshold;
        }

        m_NavMeshAgent.SetDestination(List.Value[0].transform.position);
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (m_NavMeshAgent.remainingDistance <= DistanceThreshold)
        {
            LookForward();
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (m_NavMeshAgent != null && m_NavMeshAgent.isOnNavMesh)
        {
            m_NavMeshAgent.ResetPath();
            m_NavMeshAgent.stoppingDistance = m_PreviousStoppingDistance;
        }
    }

    private void LookForward()
    {
        Vector3 targetForward = List.Value[0].transform.forward;
        Vector3 flatForward = new(targetForward.x, 0, targetForward.z);
        Quaternion targetRotation = Quaternion.LookRotation(flatForward, Vector3.up);
        Self.Value.transform.DORotateQuaternion(targetRotation, 0.5f).SetEase(Ease.OutSine);
    }
}

