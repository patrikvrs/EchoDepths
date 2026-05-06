using System;
using System.Collections.Generic;
using Godot;

public partial class SkeletonWarriorBT : BehaviourTree, IAIController
{
    private new IAIHost _host;
    private NavigationAgent3D _agent;
    private Node3D _target;
    private new Blackboard _blackboard;
    private BehaviourTree _root;
    private bool _isActive;
    private AnimationTree _animationTree;
    private AnimationNodeStateMachinePlayback _animationState;

    public void Setup(IAIHost host, Blackboard blackboard)
    {
        _host = host;
        _agent = host?.NavigationAgent;
        _target = host?.Target;

        _blackboard = blackboard ?? new Blackboard();

        BuildBehaviourTree();
        _isActive = _root != null;
        if (_host?.Self != null)
        {
            _animationTree = _host.Self.GetNodeOrNull<AnimationTree>("AnimationTree");
            if (_animationTree != null)
            {
                _animationState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/StateMachine/playback");
            }
        }
    }

    public void Tick(double delta)
    {
        if (!_isActive || _root == null || _host == null)
            return;

        _target = _host.Target;
        _blackboard.Set("Target", _target);

        NodeStatus status = _root.Execute(delta);
        _blackboard.Set("BTStatus", status);

        UpdateAnimationState();
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

        var hasTarget = new HasTarget { TargetKey = "Target" };
        var isWithinChaseDistance = new IsWithinDistance { TargetKey = "Target", Distance = 15f };

        var setNavToTarget = new SetNavigationTarget { TargetKey = "Target" };
        var moveToTarget = new MoveAlongPath();

        var isWithinAttackRange = new IsWithinDistance { TargetKey = "Target", Distance = _host.GetStat(StatsID.AttackRange) };

        var blockMeleeAttack = new BlockMeleeAttack();
        var attackTarget = new AttackTarget();

        var blockSequence = new ReactiveSequence();
        blockSequence.AddChild(hasTarget);
        blockSequence.AddChild(isWithinAttackRange);
        blockSequence.AddChild(blockMeleeAttack);

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
        engageTarget.AddChild(blockSequence);
        engageTarget.AddChild(attackSequence);
        engageTarget.AddChild(chaseSequence);

        var setPatrolTarget = new SetPatrolTarget();
        var moveAlongPatrol = new MoveAlongPath();
        var waitBetweenPoints = new Wait { WaitTime = 2.0f };

        var patrolSequence = new Sequence();
        patrolSequence.AddChild(setPatrolTarget);
        patrolSequence.AddChild(moveAlongPatrol);
        patrolSequence.AddChild(waitBetweenPoints);

        var root = new ReactiveSelector();
        root.AddChild(engageTarget);
        root.AddChild(patrolSequence);

        _root = root;
        _root.SetContext(_host, _blackboard);
    }

    private void UpdateAnimationState()
    {
        if (_animationState == null || _blackboard == null || _host == null)
            return;

        if (_host.IsDead)
        {
            SetAnimationState("SkeletonWarrior_Death_B");
            return;
        }

        if (_blackboard.TryGet("LastActionName", out string lastAction))
        {
            if (_blackboard.TryGet("IsBlocking", out bool isBlocking) && isBlocking)
            {
                SetAnimationState("SkeletonWarrior_Melee_Block");
                return;
            }

            if (lastAction == "Attacking Target")
            {
                SetAnimationState("SkeletonWarrior_Attack");
                return;
            }

            if (lastAction.Contains("Moving") || lastAction.Contains("Chasing") || lastAction.Contains("Patrolling"))
            {
                SetAnimationState("SkeletonWarrior_Walking_B");
                return;
            }

            if (lastAction.Contains("Waiting") || lastAction.Contains("Setting"))
            {
                SetAnimationState("SkeletonWarrior_Idle_B");
                return;
            }
        }
        SetAnimationState("SkeletonWarrior_Idle_B");
    }
    private void SetAnimationState(string state)
    {
        if (_animationState == null)
            return;

        if (string.IsNullOrEmpty(state))
            return;

        _animationState.Travel(state);
    }
}
