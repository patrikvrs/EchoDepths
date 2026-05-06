using Godot;
using System.Collections.Generic;

public partial class SetPatrolTarget : BehaviourTree
{
    public List<Vector3> PatrolPoints;
    public string currentPatrolPointKey = "CurrentPatrolIndex";

    public override NodeStatus Execute(double delta)
    {
        _blackboard?.Set("LastActionName", "Setting Patrol Target");

        if (_host?.Self == null || _blackboard == null || _host.NavigationAgent == null || PatrolPoints == null || PatrolPoints.Count == 0)
            return NodeStatus.Failure;

        if (!_blackboard.TryGet(currentPatrolPointKey, out int currentPatrolPoint))
        {
            currentPatrolPoint = 0;
            _blackboard.Set(currentPatrolPointKey, currentPatrolPoint);
        }

        var targetPoint = PatrolPoints[currentPatrolPoint];

        if (_host.Self.GlobalPosition.DistanceTo(targetPoint) < 1.5f)
        {
            currentPatrolPoint = (currentPatrolPoint + 1) % PatrolPoints.Count;
            _blackboard.Set(currentPatrolPointKey, currentPatrolPoint);
            targetPoint = PatrolPoints[currentPatrolPoint];
        }

        _host.NavigationAgent.TargetPosition = targetPoint;
        return NodeStatus.Success;
    }
}