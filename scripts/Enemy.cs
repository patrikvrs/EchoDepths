using Godot;

public partial class Enemy : CharacterBody3D, IDamagable
{
    
    [Export]
    public NavigationAgent3D agent;

    [Export]
    public Label3D DebugLabel;

    [Export]
    public Node3D target;

    [Export]
    public EnemyStats stats;

    private IBlackboard _blackboard;

 
    private bool isDead = false;
    public override void _Ready()
    {
        _blackboard = new DictionaryBlackboard();
        _blackboard.Set("Target", target);
        _blackboard.Set("MoveSpeed", stats.GetStat("MovementSpeed"));

    }

    public override void _PhysicsProcess(double delta)
    {
        if (target == null || agent == null) return;
        if (isDead) return;

        _blackboard.Set("Target", target);
        _blackboard.Set("MoveSpeed", stats.GetStat("MovementSpeed"));

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

        Velocity = Vector3.Zero;
        SetPhysicsProcess(false);

        QueueFree();
    }
}
