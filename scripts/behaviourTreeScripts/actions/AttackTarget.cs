using Godot;

public partial class AttackTarget : BehaviourTree
{
    public new Node3D Owner;
    public IBlackboard BB;
    public override NodeStatus Execute()
    {
        if (Owner == null || BB == null)
            return NodeStatus.Failure;

        float _attackRange = BB.TryGet("AttackRange", out float attackRange) ? attackRange : 0f;

        if (!BB.TryGet("Target", out Node3D target) || target == null)
            return NodeStatus.Failure;

        float distanceToTarget = Owner.GlobalPosition.DistanceTo(target.GlobalPosition);

        if (distanceToTarget <= _attackRange - 0.2f) // Slight buffer to ensure within range
        {
            GD.Print("Attacking the target!");
            // Here you would implement the actual attack logic, e.g., reducing health.
            return NodeStatus.Success;
        }
        else
        {
            return NodeStatus.Failure;
        }
    }
    
}