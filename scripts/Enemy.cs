using Godot;

public partial class Enemy : CharacterBody3D, IDamageable, IAIHost
{

    [Export]
    public NavigationAgent3D agent;

    [Export] public Label3D DebugLabel;

    [Export]
    public Node3D target;

    [Export]
    public EnemyStats stats;

    public Node3D Self => this;
    public NavigationAgent3D NavigationAgent => agent;
    public Node3D Target => target;
    public bool IsDead => stats == null || stats.IsDead();

    public override void _PhysicsProcess(double delta)
    {
        if(Velocity.LengthSquared() > 0f)
        {
            LookAt(GlobalPosition + Velocity, Vector3.Up);
        }
        
        MoveAndSlide();
    }


    public float GetStat(CharacterBase.StatsID stat)
    {
        return stats?.GetStat(stat) ?? 0f;
    }

    public void TakeDamage(float damage)
    {
        stats.ApplyDamage(damage);

        if (stats.IsDead())
        {
            GD.Print("Enemy has been defeated!");
            QueueFree();
        }
    }

}