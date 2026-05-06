using Godot;

public partial class AttackTarget : BehaviourTree
{
    public override NodeStatus Execute(double delta)
    {
        _blackboard?.Set("LastActionName", "Attacking Target");

        if (_blackboard == null || !_blackboard.TryGet("Target", out Node3D target) || target is not IDamageable damageable)
            return NodeStatus.Failure;

        if (_blackboard.TryGet("IsAttackOnCooldown", out bool isOnCooldown) && isOnCooldown) return NodeStatus.Failure;

        if (_host?.Self is CharacterBody3D body) body.Velocity = Vector3.Zero;

        damageable.TakeDamage(_host.GetStat(StatsID.AttackDamage));
        _blackboard.Set("IsAttackOnCooldown", true);
        return NodeStatus.Success;
    }
}