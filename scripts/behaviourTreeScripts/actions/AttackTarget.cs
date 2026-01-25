using Godot;

public partial class AttackTarget : BehaviourTree
{
    public new Node3D Owner;
    public IBlackboard BB;
    public override NodeStatus Execute(double delta)
    {
        BB.Set("LastActionName", "Attacking Target");

        if (Owner == null || BB == null)
            return NodeStatus.Failure;

        float _attackDamage = BB.TryGet("AttackDamage", out float attackDamage) ? attackDamage : 0f;

        if (!BB.TryGet("Target", out Node3D target) || target == null)
            return NodeStatus.Failure;

        if (target is not IDamagable damageable)
            return NodeStatus.Failure;

        damageable.ApplyDamage(_attackDamage);
        return NodeStatus.Success;
    }
}