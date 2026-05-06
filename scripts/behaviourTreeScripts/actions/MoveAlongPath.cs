using Godot;

public partial class MoveAlongPath : BehaviourTree
{
    public override NodeStatus Execute(double delta)
    {
        _blackboard?.Set("LastActionName", "Moving Along Path");

        if (_host == null || _host.Self is not CharacterBody3D body || _host.NavigationAgent == null || _blackboard == null)
            return NodeStatus.Failure;

        if (_host.NavigationAgent.IsNavigationFinished())
        {
            body.Velocity = Vector3.Zero;
            return NodeStatus.Success;
        }

        Vector3 nextPos = _host.NavigationAgent.GetNextPathPosition();
        Vector3 dir = nextPos - body.GlobalPosition;
        dir.Y = 0f;

        if (dir.Length() < 0.1f) return NodeStatus.Running;

        dir = dir.Normalized();
        body.Velocity = dir * _host.GetStat(StatsID.MovementSpeed);

        return NodeStatus.Running;
    }
}