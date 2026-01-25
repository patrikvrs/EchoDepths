using System;
using System.Collections.Generic;
using Godot;

public partial class EnemyAI : BehaviourTree
{
    private Enemy _host;
    private NavigationAgent3D _agent;
    private Label3D _debugLabel;
    private Node3D _target;
    private EnemyStats _stats;
    private Vector3[] _patrolPoints = Array.Empty<Vector3>();
    private IBlackboard _blackboard;
    private BehaviourTree _root;
    private bool _isActive;

    public void Setup(Enemy host, NavigationAgent3D agent, Node3D target, EnemyStats stats, Vector3[] patrolPoints, Label3D debugLabel)
    {
        _host = host;
        _agent = agent;
        _target = target;
        _stats = stats;
        _patrolPoints = patrolPoints ?? Array.Empty<Vector3>();
        _debugLabel = debugLabel;

        _blackboard = new DictionaryBlackboard();
        _blackboard.Set("Target", _target);
        RefreshBlackboardStats();

        BuildBehaviourTree();
        _isActive = true;
    }

    public void Tick(double delta)
    {
        if (!_isActive || _root == null || _host == null)
            return;

        _blackboard.Set("Target", _target);
        RefreshBlackboardStats();

        var status = _root.Execute(delta);

        if (_debugLabel != null && (_blackboard.TryGet("LastActionName", out string lastActionName) ? lastActionName : "None") != "")
            _debugLabel.Text = $"BT: {lastActionName} -> {status}";
    }

    public void Stop()
    {
        _isActive = false;
        _root?.Reset();
    }

    private void RefreshBlackboardStats()
    {
        if (_stats == null || _blackboard == null)
            return;

        _blackboard.Set("MoveSpeed", _stats.GetStat("MovementSpeed"));
        _blackboard.Set("AttackRange", _stats.GetStat("AttackRange"));
        _blackboard.Set("AttackDamage", _stats.GetStat("AttackDamage"));
    }

    private void BuildBehaviourTree()
    {
        if (_host == null)
            return;

        var hasTarget = new HasTarget { Owner = _host, BB = _blackboard, TargetKey = "Target" };
        var isWithinChaseDistance = new IsWithinDistance { Owner = _host, BB = _blackboard, TargetKey = "Target", Distance = 15f };

        var setNavToTarget = new SetNavigationTarget { Owner = _host, BB = _blackboard, TargetKey = "Target", NavAgent = _agent };
        var moveToTarget = new MoveAlongPath { Owner = _host, NavAgent = _agent, BB = _blackboard };

        var isWithinAttackRange = new IsWithinDistance{Owner = _host, BB = _blackboard, TargetKey = "Target", Distance = _blackboard.TryGet("AttackRange", out float attackRange) ? attackRange + 5f : 0f };

        var attackTarget = new AttackTarget { Owner = _host, BB = _blackboard };

        var chaseSequence = new ReactiveSequence();
        chaseSequence.AddChild(hasTarget);
        chaseSequence.AddChild(isWithinChaseDistance);
        chaseSequence.AddChild(setNavToTarget);
        chaseSequence.AddChild(moveToTarget);
        chaseSequence.AddChild(isWithinAttackRange);
        chaseSequence.AddChild(attackTarget);

        var patrolPoints = new List<Vector3>(_patrolPoints);
        var setPatrolTarget = new SetPatrolTarget { Owner = _host, BB = _blackboard, NavAgent = _agent, PatrolPoints = patrolPoints };
        var moveAlongPatrol = new MoveAlongPath { Owner = _host, NavAgent = _agent, BB = _blackboard };
        var waitBetweenPoints = new Wait { WaitTime = 2.0f , BB = _blackboard };

        var patrolSequence = new Sequence();
        patrolSequence.AddChild(setPatrolTarget);
        patrolSequence.AddChild(moveAlongPatrol);
        patrolSequence.AddChild(waitBetweenPoints);

        var root = new ReactiveSelector();
        root.AddChild(chaseSequence);
        root.AddChild(patrolSequence);

        _root = root;
    }
}
