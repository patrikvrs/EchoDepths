using Godot;

public partial class Enemy : CharacterBody3D, IDamageable, IAIHost
{

    [Export]
    public NavigationAgent3D agent;

    [Export]
    public Node3D target;

    [Export]
    public EnemyStats stats;

    [Export]
    public float RotationLerpSpeed = 10.0f;

    public Node3D Self => this;
    public NavigationAgent3D NavigationAgent => agent;
    public Node3D Target => target;
    public bool IsDead => stats == null || stats.IsDead();

    public override void _PhysicsProcess(double delta)
    {
        var planarVelocity = new Vector3(Velocity.X, 0f, Velocity.Z);
        if (planarVelocity.LengthSquared() > 0.0001f)
        {
            float desiredYaw = Mathf.Atan2(planarVelocity.X, planarVelocity.Z);
            float currentYaw = Rotation.Y;
            float smoothedYaw = Mathf.LerpAngle(currentYaw, desiredYaw, RotationLerpSpeed * (float)delta);
            Rotation = new Vector3(Rotation.X, smoothedYaw, Rotation.Z);
        }

        MoveAndSlide();
    }


    public float GetStat(StatsID stat)
    {
        return stats?.GetStat(stat) ?? 0f;
    }

    public void TakeDamage(float damage)
    {
        if (stats == null)
        {
            GD.PrintErr("Stats is null. Cannot take damage.");
            return;
        }

        stats.ApplyDamage(damage);

        if (stats.IsDead())
        {
            GD.Print("Enemy has been defeated!");
            QueueFree();
        }
    }

}