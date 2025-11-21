using Godot;

public partial class EnemyPathfinder : CharacterBody3D
{
    public const float Speed = 4.0f;
    
    [Export]
    public NavigationAgent3D agent;

    [Export]
    public Node3D target;

    public override void _Ready()
    {
        agent.TargetPosition = target.GlobalTransform.Origin;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (target == null || agent == null) return;

        agent.TargetPosition = target.GlobalTransform.Origin;

        if (agent.IsNavigationFinished()) return;

        Vector3 nextPathPosition = agent.GetNextPathPosition();
        Vector3 direction = (nextPathPosition - GlobalTransform.Origin).Normalized();

        Vector3 velocity = Velocity;
        velocity.X = direction.X * Speed;
        velocity.Z = direction.Z * Speed;
        Velocity = velocity;

        MoveAndSlide();
    }
}
