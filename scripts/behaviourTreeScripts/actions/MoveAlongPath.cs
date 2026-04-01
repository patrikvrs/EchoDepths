using Godot;

public partial class MoveAlongPath : BehaviourTree
{
    public new Node3D Owner;
    public NavigationAgent3D NavAgent;
    public Blackboard BB;

    public float movementSpeed;

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

        var nextPos = NavAgent.GetNextPathPosition();
        var dir = nextPos - body.GlobalPosition;
        dir.Y = 0f;

        if (dir.Length() < 0.1f)
            return NodeStatus.Running;

        body.Velocity = dir.Normalized() * movementSpeed;

        return NodeStatus.Running;
    }

    public override void Reset()
    {
        if (Owner is CharacterBody3D body)
            body.Velocity = Vector3.Zero;
    }
}