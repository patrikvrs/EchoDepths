using Godot;

public partial class Enemy : CharacterBody3D, IDamageable
{

    [Export]
    public NavigationAgent3D agent;

    [Export] public Label3D DebugLabel;

    [Export]
    public Node3D target;

    [Export]
    public EnemyStats stats;

    [Export]
    public Vector3[] patrolPoints = new Vector3[]
    {
        new Vector3(-10, 0.5f, -20),
        new Vector3(10, 0.5f, 0),
        new Vector3(-10, 0.5f, 0)
    };

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