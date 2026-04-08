using System;
using System.Collections.Generic;
using Godot;

public partial class SimpleEnemy_BT : BehaviourTree, IAIController
{
    [Export]
    private Vector3[] patrolPoints =
    {
        new Vector3(-10, 0.5f, -20),
        new Vector3(10, 0.5f, 0),
        new Vector3(-10, 0.5f, 0)
    };

    private IAIHost _host;
    private NavigationAgent3D _agent;
    private Node3D _target;
    private Vector3[] _patrolPoints = Array.Empty<Vector3>();
    private Blackboard _blackboard;
    private BehaviourTree _root;
    private bool _isActive;

    public void Setup(IAIHost host, Blackboard blackboard)
    {
        _host = host;
        _agent = host?.NavigationAgent;
        _target = host?.Target;
        _patrolPoints = patrolPoints ?? Array.Empty<Vector3>();

        _blackboard = blackboard ?? new Blackboard();

        BuildBehaviourTree();
        _isActive = _root != null;
    }

    public void Tick(double delta)
    {
        if (!_isActive || _root == null || _host == null)
            return;

        _target = _host.Target;
        _blackboard.Set("Target", _target);

        NodeStatus status = _root.Execute(delta);
        _blackboard.Set("BTStatus", status);
    }

    public void Stop()
    {
        _isActive = false;
        _root?.Reset();
    }

    private void BuildBehaviourTree()
    {
        if (_host == null)
            return;

        var hasTarget = new HasTarget { Owner = _host.Self, BB = _blackboard, TargetKey = "Target" };
        var isWithinChaseDistance = new IsWithinDistance { Owner = _host.Self, BB = _blackboard, TargetKey = "Target", Distance = 15f };

        var setNavToTarget = new SetNavigationTarget { Owner = _host.Self, BB = _blackboard, TargetKey = "Target", NavAgent = _agent };
        var moveToTarget = new MoveAlongPath { Owner = _host, NavAgent = _agent, BB = _blackboard };

        var isWithinAttackRange = new IsWithinDistance { Owner = _host.Self, BB = _blackboard, TargetKey = "Target", Distance = _host.GetStat(StatsID.AttackRange) };

        var attackTarget = new AttackTarget { Owner = _host, BB = _blackboard };

        var chaseSequence = new ReactiveSequence();
        chaseSequence.AddChild(hasTarget);
        chaseSequence.AddChild(isWithinChaseDistance);
        chaseSequence.AddChild(setNavToTarget);
        chaseSequence.AddChild(moveToTarget);

        var attackSequence = new ReactiveSequence();
        attackSequence.AddChild(hasTarget);
        attackSequence.AddChild(isWithinAttackRange);
        attackSequence.AddChild(attackTarget);

        var engageTarget = new ReactiveSelector();
        engageTarget.AddChild(attackSequence);
        engageTarget.AddChild(chaseSequence);

        var patrolPoints = new List<Vector3>(_patrolPoints);
        var setPatrolTarget = new SetPatrolTarget { Owner = _host, BB = _blackboard, NavAgent = _agent, PatrolPoints = patrolPoints };
        var moveAlongPatrol = new MoveAlongPath { Owner = _host, NavAgent = _agent, BB = _blackboard };
        var waitBetweenPoints = new Wait { WaitTime = 2.0f, BB = _blackboard };

        var patrolSequence = new Sequence();
        patrolSequence.AddChild(setPatrolTarget);
        patrolSequence.AddChild(moveAlongPatrol);
        patrolSequence.AddChild(waitBetweenPoints);

        var root = new ReactiveSelector();
        root.AddChild(engageTarget);
        root.AddChild(patrolSequence);

        _root = root;
    }
}
