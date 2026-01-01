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

    private IBlackboard _blackboard;

    private BehaviourTree _root;
 
    private bool isDead = false;
    public override void _Ready()
    {
        _blackboard = new DictionaryBlackboard();
        _blackboard.Set("Target", target);
        _blackboard.Set("MoveSpeed", stats.GetStat("MovementSpeed"));
        BuildBehaviourTree();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (isDead) return;

        _blackboard.Set("Target", target);
        _blackboard.Set("MoveSpeed", stats.GetStat("MovementSpeed"));

        var status = _root?.Execute() ?? BehaviourTree.NodeStatus.Failure;

        if (DebugLabel != null && _root != null)
            DebugLabel.Text = $"BT: {_root.GetType().Name} -> {status}";
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

    private void BuildBehaviourTree()
    {
        var hasTarget = new HasTarget { Owner = this, BB = _blackboard, TargetKey = "Target" };
        var isWithinDistance = new IsWithinDistance { Owner = this, BB = _blackboard, TargetKey = "Target", Distance = 10f };
        var setNav = new SetNavigationTarget { Owner = this, BB = _blackboard, TargetKey = "Target", NavAgent = agent };
        var move = new MoveAlongPath { Owner = this, NavAgent = agent, BB = _blackboard };

        var chase = new ReactiveSequence();
        chase.AddChild(hasTarget);
        chase.AddChild(isWithinDistance);
        chase.AddChild(setNav);
        chase.AddChild(move);

        _root = chase;
    }
}
