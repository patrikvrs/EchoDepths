using Godot;
using System.Collections.Generic;

public partial class SetPatrolTarget : BehaviourTree
{
    public new Node3D Owner;
    public Blackboard BB;
    public NavigationAgent3D NavAgent;
    public List<Vector3> PatrolPoints;
    public string currentPatrolPointKey = "CurrentPatrolIndex";

    public override NodeStatus Execute(double delta)
    {
        BB?.Set("LastActionName", "Setting Patrol Target");

        if (Owner == null || BB == null || NavAgent == null || PatrolPoints == null || PatrolPoints.Count == 0)
            return NodeStatus.Failure;

        if (!BB.TryGet(currentPatrolPointKey, out int currentPatrolPoint))
        {
            currentPatrolPoint = 0;
            BB.Set(currentPatrolPointKey, currentPatrolPoint);
        }

        var targetPoint = PatrolPoints[currentPatrolPoint];

        if (Owner.GlobalPosition.DistanceTo(targetPoint) < 1.5f)
        {
            currentPatrolPoint = (currentPatrolPoint + 1) % PatrolPoints.Count;
            BB.Set(currentPatrolPointKey, currentPatrolPoint);
            targetPoint = PatrolPoints[currentPatrolPoint];
        }

        NavAgent.TargetPosition = targetPoint;
        return NodeStatus.Success;
    }
}