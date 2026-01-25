using Godot;

public partial class MoveAlongPath : BehaviourTree
{
    public new Node3D Owner;
    public NavigationAgent3D NavAgent;
    public IBlackboard BB;

    public override NodeStatus Execute(double delta)
    {

        BB.Set("LastActionName", "Moving Along Path");

        if (Owner == null || NavAgent == null || BB == null)
            return NodeStatus.Failure;

        if (NavAgent.IsNavigationFinished())
            return NodeStatus.Success;

        if (Owner is not CharacterBody3D body)
            return NodeStatus.Failure;


        var nextPos = NavAgent.GetNextPathPosition();
        var dir = nextPos - body.GlobalPosition;
        dir.Y = 0f;

        if (dir.Length() < 0.1f)
            return NodeStatus.Running;

        if (BB.TryGet<float>("MoveSpeed", out var speed))
            body.Velocity = dir.Normalized() * speed;

        body.LookAt(body.GlobalPosition + dir, Vector3.Up);
        body.MoveAndSlide();

        return NodeStatus.Running;
    }

    public override void Reset()
    {
        if (Owner is CharacterBody3D body)
            body.Velocity = Vector3.Zero;
    }
}