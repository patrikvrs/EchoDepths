using Godot;

public partial class MoveAwayFromTarget : BehaviourTree
{
    public string TargetKey = "Target";
    public float RetreatDistance = 10f;

    public override NodeStatus Execute(double delta)
    {
        _blackboard?.Set("LastActionName", "Retreating From Target");

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
        Vector3 retreatTarget = _host.Self.GlobalPosition + awayDirection * RetreatDistance;
        _host.NavigationAgent.TargetPosition = retreatTarget;

        if (_host.NavigationAgent.IsNavigationFinished())
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