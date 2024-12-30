using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DG.Tweening;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "LookTo", story: "[Self] looks to [ForwardVector]", category: "Action", id: "137cb05bd42b2740bb8f9d45d67be0f6")]
public partial class LookToAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> ForwardVector;

    protected override Status OnStart()
    {
        Vector3 flatForward = new(ForwardVector.Value.x, 0, ForwardVector.Value.z);
        Quaternion targetRotation = Quaternion.LookRotation(flatForward, Vector3.up);
        Self.Value.transform.DORotateQuaternion(targetRotation, 0.5f).SetEase(Ease.OutSine);
        return Status.Success;
    }
}

