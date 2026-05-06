using Godot;

public partial class BlockMeleeAttack : BehaviourTree
{
    public override NodeStatus Execute(double delta)
    {
        _blackboard?.Set("LastActionName", "Blocking Melee Attack");

        if (_blackboard == null || _host?.Self == null)
            return NodeStatus.Failure;

        if (_blackboard.TryGet("IsBlockOnCooldown", out bool isOnCooldown) && isOnCooldown)
            return NodeStatus.Failure;

        if (!_blackboard.TryGet("Target", out Node3D target) || target == null)
            return NodeStatus.Failure;

        Vector3 toTarget = target.GlobalPosition - _host.Self.GlobalPosition;
        toTarget.Y = 0;
        if (toTarget.LengthSquared() <= 0.0001f)
            return NodeStatus.Failure;

        Vector3 forward = _host.Self.GlobalTransform.Basis.Z;
        forward.Y = 0;
        if (forward.LengthSquared() <= 0.0001f)
            return NodeStatus.Failure;

        toTarget = toTarget.Normalized();
        forward = forward.Normalized();

        // Only block if the target is in front of the enemy.
        if (toTarget.Dot(forward) <= 0.5f)
            return NodeStatus.Failure;

        if (_host.Self is CharacterBody3D body)
            body.Velocity = Vector3.Zero;

        _blackboard.Set("IsBlockOnCooldown", true);
        _blackboard.Set("IsBlocking", true);

        if (_host.Self is Node hostNode)
        {
            Timer blockTimer = new Timer();
            blockTimer.OneShot = true;
            blockTimer.WaitTime = 0.5f;
            blockTimer.Timeout += () =>
            {
                _blackboard.Set("IsBlocking", false);
                blockTimer.QueueFree();
            };
            hostNode.AddChild(blockTimer);
            blockTimer.Start();

            Timer cooldownTimer = new Timer();
            cooldownTimer.OneShot = true;
            cooldownTimer.WaitTime = 1.0f;
            cooldownTimer.Timeout += () =>
            {
                _blackboard.Set("IsBlockOnCooldown", false);
                cooldownTimer.QueueFree();
            };
            hostNode.AddChild(cooldownTimer);
            cooldownTimer.Start();
        }
        return NodeStatus.Success;
    }
}