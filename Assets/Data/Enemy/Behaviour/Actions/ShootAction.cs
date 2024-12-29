using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Shoot", story: "[Enemy] shoots [target]", category: "Action", id: "ab8ae2b981d288089acc54c1abd0bcf3")]
public partial class ShootAction : Action
{
    [SerializeReference] public BlackboardVariable<EnemyController> Enemy;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnUpdate()
    {
        Enemy.Value.Shoot(Target.Value.transform);
        return Status.Running;
    }


}

