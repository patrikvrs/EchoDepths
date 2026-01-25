using Godot;

public partial class Enemy : CharacterBody3D, IDamagable
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

    private EnemyAI _ai;
 
    private bool isDead = false;
    public override void _Ready()
    {
        foreach (Node child in GetChildren())
        {
            if (child is EnemyAI existing)
            {
                _ai = existing;
                break;
            }
        }

        if (_ai == null)
        {
            _ai = new EnemyAI();
            _ai.Name = nameof(EnemyAI);
            AddChild(_ai);
        }

        _ai.Setup(this, agent, target, stats, patrolPoints, DebugLabel);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (isDead) return;

        _ai?.Tick(delta);
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

        _ai?.Stop();
        Velocity = Vector3.Zero;
        SetPhysicsProcess(false);

        QueueFree();
    }
}