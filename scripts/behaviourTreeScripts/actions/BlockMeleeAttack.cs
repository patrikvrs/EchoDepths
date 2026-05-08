using Godot;

public partial class BlockMeleeAttack : BehaviourTree
{
    private const int BlockThreshold = 3;
    private const float BlockDuration = 2.5f;

    public override NodeStatus Execute(double delta)
    {
        _blackboard?.Set("LastActionName", "Blocking Melee Attack");

        if (_blackboard == null || _host?.Self == null || _host.Target == null || _host.Self is not Enemy enemy)
            return NodeStatus.Failure;

        if (_blackboard.TryGet("IsBlocking", out bool isBlocking) && isBlocking)
        {
            enemy.Velocity = Vector3.Zero;
            return NodeStatus.Running;
        }

        if (_blackboard.TryGet("IsBlockOnCooldown", out bool isOnCooldown) && isOnCooldown)
            return NodeStatus.Failure;

        if (!_blackboard.TryGet("HitCount", out int hitCount) || hitCount < BlockThreshold)
            return NodeStatus.Failure;


        _blackboard.Set("IsBlocking", true);
        _blackboard.Set("IsBlockOnCooldown", true);
        _blackboard.Set("HitCount", 0);


        Vector3 dirToTarget = (enemy.Target.GlobalPosition - enemy.GlobalPosition);
        dirToTarget.Y = 0;

        if (dirToTarget.LengthSquared() > 0.01f)
        {
            Vector3 normalizedDir = dirToTarget.Normalized();


            enemy.LogicalForward = normalizedDir;


            float desiredYaw = Mathf.Atan2(normalizedDir.X, normalizedDir.Z);
            enemy.Rotation = new Vector3(enemy.Rotation.X, desiredYaw, enemy.Rotation.Z);


            enemy.ForceUpdateTransform();
        }

        enemy.Velocity = Vector3.Zero;

        Timer blockTimer = new Timer();
        blockTimer.WaitTime = BlockDuration;
        blockTimer.OneShot = true;
        blockTimer.Timeout += () =>
        {
            _blackboard.Set("IsBlocking", false);
            blockTimer.QueueFree();
        };
        enemy.AddChild(blockTimer);
        blockTimer.Start();

        Timer cooldownTimer = new Timer();
        cooldownTimer.WaitTime = BlockDuration;
        cooldownTimer.OneShot = true;
        cooldownTimer.Timeout += () =>
        {
            _blackboard.Set("IsBlockOnCooldown", false);
            cooldownTimer.QueueFree();
        };
        enemy.AddChild(cooldownTimer);
        cooldownTimer.Start();

        return NodeStatus.Success;
    }
}