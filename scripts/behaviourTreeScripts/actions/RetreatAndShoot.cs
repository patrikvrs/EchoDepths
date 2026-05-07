using Godot;

public partial class RetreatAndShoot : BehaviourTree
{
    public string TargetKey = "Target";
    public string ProjectileScenePath = "res://scenes/arrow_projectile.tscn";
    public float ProjectileSpeed = 20f;
    public float ProjectileLifetime = 5f;
    public float SpawnForwardOffset = 1.2f;
    public float SpawnUpOffset = 0.6f;
    public float AimHeightOffset = 1.2f;
    public float RetreatDistance = 10f;

    public override NodeStatus Execute(double delta)
    {
        _blackboard?.Set("LastActionName", "Retreating and Shooting");

        if (_host?.Self == null || _blackboard == null || string.IsNullOrEmpty(TargetKey) || _host.NavigationAgent == null)
            return NodeStatus.Failure;

        if (!_blackboard.TryGet(TargetKey, out Node3D target) || target == null)
            return NodeStatus.Failure;

        if (_host.Self is not CharacterBody3D body)
            return NodeStatus.Failure;

        Vector3 awayDirection = (_host.Self.GlobalPosition - target.GlobalPosition);
        awayDirection.Y = 0f;

        if (awayDirection.LengthSquared() <= 0.0001f)
        {
            body.Velocity = Vector3.Zero;
            return NodeStatus.Running;
        }

        awayDirection = awayDirection.Normalized();
        Vector3 theoreticalRetreatPosition = _host.Self.GlobalPosition + awayDirection * RetreatDistance;
        Vector3 snappedRetreatPosition = theoreticalRetreatPosition;

        Rid navMap = _host.NavigationAgent.GetNavigationMap();
        if (navMap.IsValid)
        {
            snappedRetreatPosition = NavigationServer3D.MapGetClosestPoint(navMap, snappedRetreatPosition);
        }

        _host.NavigationAgent.TargetPosition = snappedRetreatPosition;

        float attackRange = _host.GetStat(StatsID.AttackRange);
        float distanceToTarget = _host.Self.GlobalPosition.DistanceTo(target.GlobalPosition);

        bool hasLOS = false;
        Vector3 from = _host.Self.GlobalPosition + Vector3.Up * 1.2f;
        Vector3 to = target.GlobalPosition + Vector3.Up * 1.2f;
        PhysicsRayQueryParameters3D ray = PhysicsRayQueryParameters3D.Create(from, to);
        ray.CollideWithAreas = false;
        ray.CollideWithBodies = true;
        ray.CollisionMask = 1 << 0; // map only
        var space = _host.Self.GetWorld3D().DirectSpaceState;
        var result = space.IntersectRay(ray);

        if (result.Count == 0)
        {
            hasLOS = true;
        }
        else if (result.ContainsKey("collider"))
        {
            Node colliderObj = (Node)result["collider"];
            Node cur = colliderObj;
            while (cur != null)
            {
                if (cur.GetInstanceId() == target.GetInstanceId())
                {
                    hasLOS = true;
                    break;
                }
                cur = cur.GetParent();
            }
        }

        if (distanceToTarget <= attackRange && hasLOS)
        {
            if (!(_blackboard.TryGet("IsAttackOnCooldown", out bool isOnCooldown) && isOnCooldown))
            {
                PackedScene arrowScene = GD.Load<PackedScene>(ProjectileScenePath);
                if (arrowScene != null)
                {
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

                    float attackSpeed = _host.GetStat(StatsID.AttackSpeed);
                    float cooldown = attackSpeed > 0f ? 1f / attackSpeed : 1f;
                    _blackboard.Set("IsAttackOnCooldown", true);

                    if (_host.Self is Node hostNode)
                    {
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
                }
            }
        }

        Vector3 finalPathPosition = _host.NavigationAgent.GetFinalPosition();
        float distanceToWall = body.GlobalPosition.DistanceTo(finalPathPosition);

        if (_host.NavigationAgent.IsNavigationFinished() || distanceToWall < 0.5f)
        {
            body.Velocity = Vector3.Zero;
            return NodeStatus.Success;
        }

        Vector3 nextPosition = _host.NavigationAgent.GetNextPathPosition();
        Vector3 direction = nextPosition - body.GlobalPosition;
        direction.Y = 0f;

        if (direction.LengthSquared() <= 0.0001f)
        {
            body.Velocity = Vector3.Zero;
            return NodeStatus.Running;
        }

        body.Velocity = direction.Normalized() * _host.GetStat(StatsID.MovementSpeed);
        return NodeStatus.Running;
    }
}