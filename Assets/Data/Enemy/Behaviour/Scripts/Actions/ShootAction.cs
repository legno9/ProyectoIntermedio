using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Shoot", story: "[EnemyController] shoots [target]", category: "Action", id: "74a6d85027a24ec645df43de0717ca83")]
public partial class ShootAction : Action
{
    [SerializeReference] public BlackboardVariable<EnemyController> EnemyController;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnUpdate()
    {
        EnemyController.Value.Shoot(Target.Value.transform);
        return Status.Running;
    }


}

