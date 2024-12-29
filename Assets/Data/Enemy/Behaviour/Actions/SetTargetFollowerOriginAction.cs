using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set TargetFollower origin", story: "Get shoot point from [EnemyController] and set as [TargetFollower] origin.", category: "Action", id: "f21e1d4cdd1b2d537b1a80b99c4abc7a")]
public partial class SetTargetFollowerOriginAction : Action
{
    [SerializeReference] public BlackboardVariable<EnemyController> EnemyController;
    [SerializeReference] public BlackboardVariable<TargetFollower> TargetFollower;
    protected override Status OnStart()
    {
        TargetFollower.Value.SetOrigin(EnemyController.Value.GetCurrentWeaponShootPoint());
        return Status.Success;
    }
}

