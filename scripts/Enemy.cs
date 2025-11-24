using Godot;

public partial class Enemy : CharacterBody3D, IDamagable
{
    public const float Speed = 4.0f;
    
    [Export]
    public NavigationAgent3D agent;

    [Export]
    public Node3D target;

    [Export]
    public EnemyStats stats;

    // added flag to avoid multiple death runs
    private bool isDead = false;

    public override void _Ready()
    {
        agent.TargetPosition = target.GlobalTransform.Origin;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (target == null || agent == null) return;
        if (isDead) return;

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

    public void ApplyDamage(float damage)
    {
        if (stats != null && !isDead)
        {
            stats.ModifyStat("CurrentHealth", -damage);
            float current = stats.GetStat("CurrentHealth");
            GD.Print($"Enemy took {damage} damage. Current Health: {current}");
            if (current <= 0f)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        GD.Print("Enemy died.");

        // stop movement/physics
        Velocity = Vector3.Zero;
        SetPhysicsProcess(false);

        // TODO: play death animation, emit a 'died' signal, drop loot, notify manager, or delay QueueFree()
        QueueFree();
    }
}
