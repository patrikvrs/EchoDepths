using Godot;

public partial class SetPatrolTarget : BehaviourTree
{
    private const float PatrolRadius = 15f;
    public override NodeStatus Execute(double delta)
    {
        _blackboard?.Set("LastActionName", "Setting Patrol Target");

        if (_host == null || _host.NavigationAgent == null)
            return NodeStatus.Failure;

        Vector3 hostPosition = _host.Self.GlobalPosition;
        Vector3 randomDirection = new Vector3((float)(GD.Randf() * 2 - 1), 0, (float)(GD.Randf() * 2 - 1)).Normalized();
        Vector3 targetPoint = hostPosition + randomDirection * PatrolRadius;

        _host.NavigationAgent.TargetPosition = targetPoint;
        return NodeStatus.Success;
    }
}