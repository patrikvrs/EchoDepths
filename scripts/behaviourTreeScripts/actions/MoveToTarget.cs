using Godot;
using System;

public partial class MoveToTarget : BehaviourTree
{
    public NavigationAgent3D Agent;
    public new CharacterBody3D Owner;

    public IBlackboard BB;
    public string TargetKey = "Target";
    public string MoveSpeedKey = "MoveSpeed";
    public float StopDistance = 1.5f;

    public override NodeStatus Execute()
    {
        if (Agent == null || Owner == null || BB == null)
            return NodeStatus.Failure;

        if (!BB.TryGet<Node3D>(TargetKey, out var target) || target == null)
            return NodeStatus.Failure;

        float dist = Owner.GlobalPosition.DistanceTo(target.GlobalPosition);
        if (dist <= StopDistance)
            return NodeStatus.Success;

        Agent.TargetPosition = target.GlobalPosition;
        if (Agent.IsNavigationFinished())
            return NodeStatus.Running;

        Vector3 next = Agent.GetNextPathPosition();
        Vector3 dir = (next - Owner.GlobalPosition).Normalized();

        float speed = BB.TryGet<float>(MoveSpeedKey, out var s) ? s : 0f;

        Owner.Velocity = new Vector3(dir.X * speed, Owner.Velocity.Y, dir.Z * speed);
        Owner.MoveAndSlide();
        return NodeStatus.Running;
    }
}
