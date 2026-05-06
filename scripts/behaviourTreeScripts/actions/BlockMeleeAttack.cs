using Godot;

public partial class BlockMeleeAttack : BehaviourTree
{
    private const int BlockThreshold = 3;

    public override NodeStatus Execute(double delta)
    {
        _blackboard?.Set("LastActionName", "Blocking Melee Attack");

        if (_blackboard == null || _host?.Self == null)
            return NodeStatus.Failure;

        if (_blackboard.TryGet("IsBlocking", out bool isBlocking) && isBlocking)
        {
            if (_host.Self is CharacterBody3D blockingBody)
                blockingBody.Velocity = Vector3.Zero;

            return NodeStatus.Running;
        }

        if (_blackboard.TryGet("IsBlockOnCooldown", out bool isOnCooldown) && isOnCooldown)
            return NodeStatus.Failure;

        if (!_blackboard.TryGet("FrontHitCount", out int frontHitCount) || frontHitCount < BlockThreshold)
            return NodeStatus.Failure;

        if (_host.Self is CharacterBody3D body)
            body.Velocity = Vector3.Zero;

        _blackboard.Set("IsBlockOnCooldown", true);
        _blackboard.Set("IsBlocking", true);
        _blackboard.Set("FrontHitCount", 0);

        if (_host.Self is Node hostNode)
        {
            Timer blockTimer = new Timer();
            blockTimer.OneShot = true;
            blockTimer.WaitTime = 3.0f;
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