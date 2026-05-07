using Godot;

public partial class SkeletonArcherBT : BehaviourTree, IAIController
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
        if (!_isActive || _root == null || _host == null || _host.IsDead)
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
        var hasLineOfSight = new HasLineOfSight { TargetKey = "Target" };
        var attackTarget = new AttackTargetWithProjectile();
        attackTarget.ProjectileScenePath = "res://scenes/arrow_projectile.tscn";
        attackTarget.ProjectileSpeed = 20f;
        attackTarget.ProjectileLifetime = 5f;
        attackTarget.SpawnForwardOffset = 1.2f;
        attackTarget.SpawnUpOffset = 0.6f;
        float attackRange = _host.GetStat(StatsID.AttackRange);
        float retreatDistance = 10f;
        float closeRange = Mathf.Max(attackRange * 0.25f, 1.5f);

        var isWithinAttackRange = new IsWithinDistance { TargetKey = "Target", Distance = attackRange };
        var isWithinCloseRange = new IsWithinDistance { TargetKey = "Target", Distance = closeRange };
        var retreatFromTarget = new MoveAwayFromTarget { TargetKey = "Target", RetreatDistance = retreatDistance };

        var retreatSequence = new ReactiveSequence();
        retreatSequence.AddChild(hasTarget);
        retreatSequence.AddChild(isWithinCloseRange);
        retreatSequence.AddChild(retreatFromTarget);

        var attackSequence = new ReactiveSequence();
        attackSequence.AddChild(hasTarget);
        attackSequence.AddChild(hasLineOfSight);
        attackSequence.AddChild(isWithinAttackRange);
        attackSequence.AddChild(attackTarget);

        var root = new ReactiveSelector();
        root.AddChild(retreatSequence);
        root.AddChild(attackSequence);

        _root = root;
        _root.SetContext(_host, _blackboard);
    }

    private void UpdateAnimationState()
    {
        if (_animationState == null || _blackboard == null)
            return;

        if (_blackboard.TryGet("LastActionName", out string lastAction))
        {
            if (lastAction == "Attacking Target")
            {
                SetAnimationState("Shoot");
                return;
            }

            if (lastAction.Contains("Moving") || lastAction.Contains("Retreating"))
            {
                SetAnimationState("Walking");
                return;
            }

            if (lastAction.Contains("Waiting") || lastAction.Contains("Setting"))
            {
                SetAnimationState("Idle");
                return;
            }
        }

        SetAnimationState("Idle");
    }

    public void SetAnimationState(string state)
    {
        if (_animationState == null || string.IsNullOrEmpty(state))
            return;

        _animationState.Travel(state);
    }
}