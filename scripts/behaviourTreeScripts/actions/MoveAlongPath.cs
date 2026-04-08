using Godot;

public partial class MoveAlongPath : BehaviourTree
{
    public new IAIHost Owner;
    public NavigationAgent3D NavAgent;
    public Blackboard BB;

    public override NodeStatus Execute(double delta)
    {
        BB?.Set("LastActionName", "Moving Along Path");

        if (Owner == null || Owner is not CharacterBody3D body || NavAgent == null || BB == null)
            return NodeStatus.Failure;

        if (NavAgent.IsNavigationFinished())
        {
            body.Velocity = Vector3.Zero;
            return NodeStatus.Success;
        }

        Vector3 nextPos = NavAgent.GetNextPathPosition();
        Vector3 dir = nextPos - body.GlobalPosition;
        dir.Y = 0f;

        if (dir.Length() < 0.1f) return NodeStatus.Running;

        dir = dir.Normalized();
        body.Velocity = dir * Owner.GetStat(StatsID.MovementSpeed);

        return NodeStatus.Running;
    }
}