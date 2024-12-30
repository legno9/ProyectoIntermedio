using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "PatrolPointsNumber", story: "[PatrolPoints] is Greater Than [number]", category: "Conditions", id: "7ead2f20cccf935607482d418ffa8fd6")]
public partial class PatrolPointsNumberCondition : Condition
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> PatrolPoints;
    [SerializeReference] public BlackboardVariable<int> Number;

    public override bool IsTrue()
    {
        return PatrolPoints.Value.Count > Number.Value;
    }
}
