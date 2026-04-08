using Godot;

public partial class AttackTarget : BehaviourTree
{
    public new IAIHost Owner;
    public Blackboard BB;

    public override NodeStatus Execute(double delta)
    {
        BB?.Set("LastActionName", "Attacking Target");

        if (BB == null || !BB.TryGet("Target", out Node3D target) || target is not IDamageable damageable)
            return NodeStatus.Failure;

        if (BB.TryGet("IsAttackOnCooldown", out bool isOnCooldown) && isOnCooldown) return NodeStatus.Failure;

        if (Owner is CharacterBody3D body) body.Velocity = Vector3.Zero;

        damageable.TakeDamage(Owner.GetStat(StatsID.AttackDamage));
        BB.Set("IsAttackOnCooldown", true);
        return NodeStatus.Success;
    }
}