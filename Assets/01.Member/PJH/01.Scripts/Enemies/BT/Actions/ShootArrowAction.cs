using Code.Enemies;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ShootArrow", story: "[Self] Shoot arrow to [Target]", category: "Action", id: "9f885b78ec44567163683686c57b4287")]
public partial class ShootArrowAction : Action
{
    [SerializeReference] public BlackboardVariable<ArcherAttackCompo> Self;
    [SerializeReference] public BlackboardVariable<Transform> Target;

    protected override Status OnStart()
    {
        if (Self.Value == null || Target.Value == null)
            return Status.Failure;

        Vector3 dir = (Target.Value.position - Self.Value.transform.position).normalized;
        
        Self.Value.ShootArrow(dir);
        
        return Status.Success;
    }
}

