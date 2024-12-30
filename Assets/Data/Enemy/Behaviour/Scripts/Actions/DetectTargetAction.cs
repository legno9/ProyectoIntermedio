using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DetectTarget", story: "Update [SightDetector] and [HeardDetector] with [target] and set [targetDetected] and [TargetFollower] if it is detected.", category: "Action", id: "8794885f98d5b810c0185fe4e978a563")]
public partial class DetectTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<SightDetector> SightDetector;
    [SerializeReference] public BlackboardVariable<HeardDetector> HeardDetector;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<bool> TargetDetected;
    [SerializeReference] public BlackboardVariable<TargetFollower> TargetFollower;
    protected override Status OnUpdate()
    {
        if (SightDetector.Value.Detect(Target.Value) || HeardDetector.Value.IsTargetDetected(Target.Value))
        {
            TargetFollower.Value.SetTarget(Target.Value.transform);
            TargetDetected.Value = true;
            return Status.Success;
        }

        TargetFollower.Value.SetTarget(null);
        TargetDetected.Value = false;
        return Status.Failure;
    }
}

