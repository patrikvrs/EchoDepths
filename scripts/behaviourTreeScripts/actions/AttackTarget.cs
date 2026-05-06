using Godot;

public partial class AttackTarget : BehaviourTree
{
    private const string AttackStateKey = "IsAttacking";

    public override NodeStatus Execute(double delta)
    {
        _blackboard?.Set("LastActionName", "Attacking Target");

        if (_blackboard == null || _host?.Self == null)
            return NodeStatus.Failure;

        if (_blackboard.TryGet(AttackStateKey, out bool isAttacking) && isAttacking)
        {
            if (_host.Self is CharacterBody3D attackingBody)
                attackingBody.Velocity = Vector3.Zero;

            return NodeStatus.Running;
        }

        if (_blackboard.TryGet("IsAttackOnCooldown", out bool isOnCooldown) && isOnCooldown)
            return NodeStatus.Failure;

        if (!_blackboard.TryGet("Target", out Node3D target) || target is not IDamageable damageable)
            return NodeStatus.Failure;

        if (_host.Self is CharacterBody3D body)
            body.Velocity = Vector3.Zero;

        if (_host.Self is Enemy self)
        {
            Vector3 directionToTarget = (target.GlobalPosition - self.GlobalPosition).Normalized();
            float desiredYaw = Mathf.Atan2(directionToTarget.X, directionToTarget.Z);
            float currentYaw = self.Rotation.Y;
            float smoothedYaw = Mathf.LerpAngle(currentYaw, desiredYaw, self.RotationLerpSpeed * (float)delta);
            self.Rotation = new Vector3(self.Rotation.X, smoothedYaw, self.Rotation.Z);
        }

        damageable.TakeDamage(_host.GetStat(StatsID.AttackDamage));

        float attackSpeed = _host.GetStat(StatsID.AttackSpeed);
        float cooldown = attackSpeed > 0f ? 1f / attackSpeed : 1f;

        _blackboard.Set(AttackStateKey, true);
        _blackboard.Set("IsAttackOnCooldown", true);

        if (_host.Self is Node hostNode)
        {
            Timer attackTimer = new Timer();
            attackTimer.OneShot = true;
            attackTimer.WaitTime = cooldown;
            attackTimer.Timeout += () =>
            {
                _blackboard.Set(AttackStateKey, false);
                attackTimer.QueueFree();
            };
            hostNode.AddChild(attackTimer);
            attackTimer.Start();

            Timer cooldownTimer = new Timer();
            cooldownTimer.OneShot = true;
            cooldownTimer.WaitTime = cooldown;
            cooldownTimer.Timeout += () =>
            {
                _blackboard.Set("IsAttackOnCooldown", false);
                cooldownTimer.QueueFree();
            };
            hostNode.AddChild(cooldownTimer);
            cooldownTimer.Start();
        }

        return NodeStatus.Success;
    }
}