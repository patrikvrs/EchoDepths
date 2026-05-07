using Godot;

public partial class AttackTargetWithProjectile : BehaviourTree
{
    private const string AttackStateKey = "IsAttacking";
    public string ProjectileScenePath = "res://scenes/arrow_projectile.tscn";
    public float ProjectileSpeed = 20f;
    public float ProjectileLifetime = 5f;
    public float SpawnForwardOffset = 1.2f;
    public float SpawnUpOffset = 0.6f;
    public float AimHeightOffset = 1.2f;

    public override NodeStatus Execute(double delta)
    {
        if (_blackboard == null || _host?.Self == null)
            return NodeStatus.Failure;

        if (_blackboard.TryGet(AttackStateKey, out bool isAttacking) && isAttacking)
        {
            _blackboard.Set("LastActionName", "Waiting For Attack Cooldown");

            if (_host.Self is CharacterBody3D attackingBody)
                attackingBody.Velocity = Vector3.Zero;

            return NodeStatus.Running;
        }

        if (_blackboard.TryGet("IsAttackOnCooldown", out bool isOnCooldown) && isOnCooldown)
        {
            _blackboard.Set("LastActionName", "Waiting For Attack Cooldown");
            return NodeStatus.Failure;
        }

        if (!_blackboard.TryGet("Target", out Node3D target) || target == null)
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

        PackedScene arrowScene = GD.Load<PackedScene>(ProjectileScenePath);
        if (arrowScene == null)
            return NodeStatus.Failure;

        Node3D arrowRoot = arrowScene.Instantiate<Node3D>();

        Vector3 hostPos = _host.Self.GlobalPosition;
        Vector3 toTarget = (target.GlobalPosition - hostPos).Normalized();
        Vector3 spawnPos = hostPos + toTarget * SpawnForwardOffset + Vector3.Up * SpawnUpOffset;

        Node parent = _host.Self.GetParent() ?? _host.Self;
        parent.AddChild(arrowRoot);

        Vector3 aimPoint = target.GlobalPosition + Vector3.Up * AimHeightOffset;

        arrowRoot.GlobalPosition = spawnPos;
        arrowRoot.LookAt(aimPoint, Vector3.Up);

        Vector3 velocity = (aimPoint - arrowRoot.GlobalPosition).Normalized() * ProjectileSpeed;
        var controller = new ProjectileController();
        controller.Damage = _host.GetStat(StatsID.AttackDamage);
        controller.ShooterId = _host.Self.GetInstanceId();
        controller.Lifetime = ProjectileLifetime;
        controller.Velocity = velocity;
        arrowRoot.AddChild(controller);

        _blackboard.Set("LastActionName", "Attacking Target");

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
