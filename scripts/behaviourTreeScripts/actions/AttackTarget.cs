using Godot;

public partial class AttackTarget : BehaviourTree
{
    public new Node3D Owner;
    public IBlackboard BB;
    public override NodeStatus Execute(double delta)
    {
        if (Owner == null || BB == null)
            return NodeStatus.Failure;

        float _attackRange = BB.TryGet("AttackRange", out float attackRange) ? attackRange : 0f;
        float _attackDamage = BB.TryGet("AttackDamage", out float attackDamage) ? attackDamage : 0f;

        if (!BB.TryGet("Target", out Node3D target) || target == null)
            return NodeStatus.Failure;

        

        float distanceToTarget = Owner.GlobalPosition.DistanceTo(target.GlobalPosition);

        if (distanceToTarget <= _attackRange - 0.2f) // Slight buffer to ensure within range
        {
            GD.Print($"Dealing {_attackDamage} damage to the target.");
            return NodeStatus.Success;
        }
        else
        {
            return NodeStatus.Failure;
        }
    }
    
}