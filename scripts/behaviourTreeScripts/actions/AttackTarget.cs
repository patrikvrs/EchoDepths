using Godot;

public partial class AttackTarget : BehaviourTree
{
    public override NodeStatus Execute(double delta)
    {
        _blackboard?.Set("LastActionName", "Attacking Target");

        if (_blackboard == null || !_blackboard.TryGet("Target", out Node3D target) || target is not IDamageable damageable)
            return NodeStatus.Failure;

        if (_blackboard.TryGet("IsAttackOnCooldown", out bool isOnCooldown) && isOnCooldown)
        {
            if (_host?.Self is CharacterBody3D body) body.Velocity = Vector3.Zero;
            return NodeStatus.Running;
        }

        if (_host?.Self is CharacterBody3D attackingBody) attackingBody.Velocity = Vector3.Zero;

        damageable.TakeDamage(_host.GetStat(StatsID.AttackDamage));

        float attackSpeed = _host.GetStat(StatsID.AttackSpeed);
        float cooldown = attackSpeed > 0f ? 1f / attackSpeed : 1f;

        _blackboard.Set("IsAttackOnCooldown", true);

        if (_host?.Self is Node hostNode)
        {
            if (!hostNode.HasNode("_attack_cooldown_timer"))
            {
                Timer t = new Timer();
                t.Name = "_attack_cooldown_timer";
                t.OneShot = true;
                t.WaitTime = cooldown;
                t.Timeout += () =>
                {
                    _blackboard.Set("IsAttackOnCooldown", false);
                    t.QueueFree();
                };
                hostNode.AddChild(t);
                t.Start();
            }
        }

        return NodeStatus.Running;
    }
}