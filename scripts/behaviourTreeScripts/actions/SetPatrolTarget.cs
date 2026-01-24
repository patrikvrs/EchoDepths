using Godot;
using System.Collections.Generic;

public partial class SetPatrolTarget : BehaviourTree
{
    public new Node3D Owner;
    public IBlackboard BB;
    public NavigationAgent3D NavAgent;
    public List<Vector3> PatrolPoints;
    public string CurrentPatrolIndexKey = "CurrentPatrolIndex";

    public override NodeStatus Execute()
    {
        if (Owner == null || BB == null || NavAgent == null || PatrolPoints == null || PatrolPoints.Count == 0)
            return NodeStatus.Failure;

        if (!BB.TryGet(CurrentPatrolIndexKey, out int currentIndex))
        {
            currentIndex = 0;
            BB.Set(CurrentPatrolIndexKey, currentIndex);
        }

        var targetPoint = PatrolPoints[currentIndex];

        if (Owner.GlobalPosition.DistanceTo(targetPoint) < 1.5f)
        {
            currentIndex = (currentIndex + 1) % PatrolPoints.Count;
            BB.Set(CurrentPatrolIndexKey, currentIndex);
            targetPoint = PatrolPoints[currentIndex];
        }

        NavAgent.TargetPosition = targetPoint;
        return NodeStatus.Success;
    }
}