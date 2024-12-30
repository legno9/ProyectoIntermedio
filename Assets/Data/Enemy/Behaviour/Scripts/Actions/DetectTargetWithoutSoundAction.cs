using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Detect target without sound", story: "Update [SightDetector] with [target] and set [targetDetected] and [TargetFollower] if it is detected.", category: "Action", id: "678bf822e8e9b083ed2f9a8c9bf09618")]
public partial class DetectTargetWithoutSoundAction : Action
{
    [SerializeReference] public BlackboardVariable<SightDetector> SightDetector;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<bool> TargetDetected;
    [SerializeReference] public BlackboardVariable<TargetFollower> TargetFollower;

    protected override Status OnUpdate()
    {
        if (SightDetector.Value.Detect(Target.Value))
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

