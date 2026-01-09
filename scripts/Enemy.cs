using Godot;
using System.Collections.Generic;

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
        _blackboard.Set("AttackRange", stats.GetStat("AttackRange"));

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
        var isWithinChaseDistance = new IsWithinDistance { Owner = this, BB = _blackboard, TargetKey = "Target", Distance = 15f };
        var isNotWithinAttackDistance = new IsWithinDistance { Owner = this, BB = _blackboard, TargetKey = "Target", Distance = _blackboard.TryGet("AttackRange", out float range) ? range : 2f };
        var invertAttackDistance = new Inverter();
        invertAttackDistance.AddChild(isNotWithinAttackDistance);
        var setNavToTarget = new SetNavigationTarget { Owner = this, BB = _blackboard, TargetKey = "Target", NavAgent = agent };
        var moveToTarget = new MoveAlongPath { Owner = this, NavAgent = agent, BB = _blackboard };

        var chaseSequence = new ReactiveSequence();
        chaseSequence.AddChild(hasTarget);
        chaseSequence.AddChild(isWithinChaseDistance);
        chaseSequence.AddChild(invertAttackDistance);
        chaseSequence.AddChild(setNavToTarget);
        chaseSequence.AddChild(moveToTarget);

        var setPatrolTarget = new SetPatrolTarget { Owner = this, BB = _blackboard, NavAgent = agent, PatrolPoints = new List<Vector3>(patrolPoints) };
        var moveAlongPatrol = new MoveAlongPath { Owner = this, NavAgent = agent, BB = _blackboard };
        
        var patrolSequence = new Sequence();
        patrolSequence.AddChild(setPatrolTarget);
        patrolSequence.AddChild(moveAlongPatrol);

        var root = new ReactiveSelector();
        root.AddChild(chaseSequence);
        root.AddChild(patrolSequence);

        _root = root;
    }
}
