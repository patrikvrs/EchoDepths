using Godot;
using System;

public partial class MoveToTarget : BehaviourTree
{
    public NavigationAgent3D Agent;
    public new CharacterBody3D Owner;
    public Func<Node3D> GetTarget;
    public Func<float> GetMoveSpeed;
    public float StopDistance = 1.5f;

    public override NodeStatus Execute()
    {
        Node3D target = GetTarget?.Invoke();
        if (Agent == null || Owner == null || target == null)
            return NodeStatus.Failure;

        float dist = Owner.GlobalPosition.DistanceTo(target.GlobalPosition);
        if (dist <= StopDistance)
            return NodeStatus.Success;

        Agent.TargetPosition = target.GlobalPosition;
        if (Agent.IsNavigationFinished())
            return NodeStatus.Running;

        Vector3 next = Agent.GetNextPathPosition();
        Vector3 dir = (next - Owner.GlobalPosition).Normalized();
        float Speed = GetMoveSpeed?.Invoke() ?? 0f;

        Owner.Velocity = new Vector3(dir.X * Speed, Owner.Velocity.Y, dir.Z * Speed);
        Owner.MoveAndSlide();
        return NodeStatus.Running;
    }
}
