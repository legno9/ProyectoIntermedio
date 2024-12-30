using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetLastLocationSeen", story: "Set [target] [lastLocationSeen]", category: "Action", id: "c6494550eeed05a3d7f5115ebbf940cf")]
public partial class SetLastLocationSeenAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<bool> Continuous = new BlackboardVariable<bool>(false);

    [SerializeReference] public BlackboardVariable<Vector3> LastLocationSeen;

    protected override Status OnStart()
    {
        UpdateLastLocationSeen();
        return Continuous.Value ? Status.Running : Status.Success;
    }
    protected override Status OnUpdate()
    {

        if (Continuous.Value)
        {
            UpdateLastLocationSeen();
            return Status.Running;
        }
        return Status.Success;
    }

    private void UpdateLastLocationSeen()
    {
        LastLocationSeen.Value = Target.Value.transform.position;
    }

}

