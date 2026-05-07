using Godot;

public partial class SkeletonMageAI : Node, IAIController
{
    private const float ChaseDistance = 15f;
    private const float RetreatDistance = 4f;
    private const float NeutralDistance = 6f;
    private const float HealRadius = 15f;
    private const float HealCooldown = 8f;

    private bool _isActive;
    private IAIHost _host;
    private Blackboard _blackboard;
    private Godot.Collections.Array<UtilityDecision> _decisions;
    private AnimationTree _animationTree;
    private AnimationNodeStateMachinePlayback _animationState;
    private float _lastAttackTime = 0f;
    private float _lastHealTime = 0f;

    public void Setup(IAIHost host, Blackboard blackboard)
    {
        _isActive = true;
        _host = host;
        _blackboard = blackboard;
        _decisions = BuildMageDecisions();

        _blackboard.Set("LastAttackTime", _lastAttackTime);
        _blackboard.Set("LastHealTime", _lastHealTime);
        _blackboard.Set("AttackJustExecuted", 0.0f);
        _blackboard.Set("HealJustExecuted", 0.0f);

        if (host?.Self != null)
        {
            _animationTree = host.Self.GetNodeOrNull<AnimationTree>("AnimationTree");
            if (_animationTree != null)
            {
                _animationState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/StateMachine/playback");
            }
        }
    }

    public void Tick(double delta)
    {
        if (!_isActive || _host == null || _blackboard == null)
            return;

        _lastAttackTime += (float)delta;
        _lastHealTime += (float)delta;

        _blackboard.Set("LastAttackTime", _lastAttackTime);
        _blackboard.Set("LastHealTime", _lastHealTime);
        _blackboard.Set("AttackJustExecuted", 0.0f);
        _blackboard.Set("HealJustExecuted", 0.0f);
        UpdateTargetState();

        UtilityDecision bestDecision = null;
        float bestScore = -1f;
        string decisionScores = string.Empty;

        foreach (UtilityDecision decision in _decisions)
        {
            if (decision == null || decision.Action == null)
                continue;

            float score = decision.Evaluate(_host, _blackboard);

            if (decisionScores.Length > 0)
                decisionScores += "\n";
            decisionScores += $"{decision.DecisionName}: {score:F2}";

            if (score > bestScore)
            {
                bestScore = score;
                bestDecision = decision;
            }
        }

        _blackboard.Set("UtilityScore", bestScore);
        _blackboard.Set("DecisionScores", decisionScores);
        if (bestDecision != null)
        {
            _blackboard.Set("BestDecisionName", bestDecision.DecisionName);
        }

        if (bestDecision != null && bestDecision.Action != null)
        {
            bestDecision.Action.SetContext(_host, _blackboard);
            bestDecision.Action.Execute();

            if (_blackboard.TryGet("AttackJustExecuted", out float attackJustExecuted) && attackJustExecuted >= 1.0f)
            {
                _lastAttackTime = 0.0f;
            }

            if (_blackboard.TryGet("HealJustExecuted", out float healJustExecuted) && healJustExecuted >= 1.0f)
            {
                _lastHealTime = 0.0f;
            }
        }

        UpdateAnimationState();

        _blackboard.Set("AttackJustExecuted", 0.0f);
        _blackboard.Set("HealJustExecuted", 0.0f);
    }

    public void Stop()
    {
        _isActive = false;
    }

    private void UpdateTargetState()
    {
        Node3D target = _host.Target;
        bool hasTarget = target != null;
        _blackboard.Set("HasTarget", hasTarget ? 1.0f : 0.0f);
        _blackboard.Set("HasNoTarget", hasTarget ? 0.0f : 1.0f);


        if (!hasTarget || _host.Self == null)
        {
            _blackboard.Set("TargetDistance", float.MaxValue);
            _blackboard.Set("ThreatLevel", 0.0f);
            _blackboard.Set("CloseRangeScore", 0.0f);
            _blackboard.Set("MidRangeScore", 0.0f);
            _blackboard.Set("FarRangeScore", 0.0f);
            _blackboard.Set("HealNeedScore", 0.0f);
            _blackboard.Set("CanAttackAgain", 0.0f);
            return;
        }

        float targetDistance = _host.Self.GlobalPosition.DistanceTo(target.GlobalPosition);

        _blackboard.Set("TargetDistance", targetDistance);

        float threatLevel = Mathf.Clamp(1.0f - (targetDistance / ChaseDistance), 0.0f, 1.0f);
        _blackboard.Set("ThreatLevel", threatLevel);

        float closeRangeScore = Mathf.Clamp(1.0f - (targetDistance / RetreatDistance), 0.0f, 1.0f);
        _blackboard.Set("CloseRangeScore", closeRangeScore);

        float midRangeScore = 0.0f;
        if (targetDistance >= NeutralDistance && targetDistance <= ChaseDistance)
        {
            midRangeScore = (targetDistance - NeutralDistance) / (ChaseDistance - NeutralDistance);
        }
        _blackboard.Set("MidRangeScore", midRangeScore);

        float farRangeScore = targetDistance > ChaseDistance ? 1.0f : 0.0f;
        _blackboard.Set("FarRangeScore", farRangeScore);

        _blackboard.Set("HealNeedScore", CalculateHealNeedScore());

        float attackSpeed = _host.GetStat(StatsID.AttackSpeed);
        float attackCooldown = attackSpeed > 0f ? 1f / attackSpeed : float.MaxValue;
        _blackboard.Set("CanAttackAgain", _lastAttackTime >= attackCooldown ? 1.0f : 0.0f);

        _blackboard.Set("CanHealAgain", _lastHealTime >= HealCooldown ? 1.0f : 0.0f);
    }

    private void UpdateAnimationState()
    {
        if (_animationState == null || _blackboard == null)
            return;

        bool firedThisTick = _blackboard.TryGet("AttackJustExecuted", out float attackJustExecuted) && attackJustExecuted >= 1.0f;
        bool healedThisTick = _blackboard.TryGet("HealJustExecuted", out float healJustExecuted) && healJustExecuted >= 1.0f;
        if (firedThisTick || healedThisTick)
        {
            SetAnimationState("Magic_Shoot");
            return;
        }

        if (_blackboard.TryGet("LastActionName", out string lastAction))
        {
            if (lastAction == "Casting Fireball")
            {
                if (_host.Self is CharacterBody3D body && _host.Target is Node3D target)
                {
                    var directionToTarget = (target.GlobalPosition - body.GlobalPosition).Normalized();
                    directionToTarget.Y = 0f;
                    var targetRotation = new Vector3(0, Mathf.Atan2(directionToTarget.X, directionToTarget.Z), 0);
                    body.GlobalRotation = targetRotation;
                }
                SetAnimationState("Magic_Shoot");
                return;
            }

            if (lastAction.Contains("Healing"))
            {
                SetAnimationState("Magic_Heal");
                return;
            }

            if (lastAction.Contains("Chasing") || lastAction.Contains("Retreating"))
            {
                SetAnimationState("Walking");
                return;
            }
        }

        SetAnimationState("Idle");
    }

    private void SetAnimationState(string state)
    {
        if (_animationState == null || string.IsNullOrEmpty(state))
            return;

        try
        {
            _animationState.Travel(state);
        }
        catch
        {
            _animationState.Travel("Idle");
        }
    }

    private Godot.Collections.Array<UtilityDecision> BuildMageDecisions()
    {
        var decisions = new Godot.Collections.Array<UtilityDecision>();
        decisions.Add(CreateRetreatDecision());
        decisions.Add(CreateHealDecision());
        decisions.Add(CreateFireballDecision());
        decisions.Add(CreateChaseDecision());
        decisions.Add(CreateWaitDecision());
        return decisions;
    }

    private UtilityDecision CreateRetreatDecision()
    {
        var decision = new UtilityDecision { DecisionName = "Retreat", Action = new RetreatAction { ActionName = "Retreating" } };
        decision.Considerations.Add(CreateConsideration("CloseRangeScore", new Vector2(0.0f, 0.0f), new Vector2(0.2f, 0.5f), new Vector2(1.0f, 1.0f)));
        return decision;
    }

    private UtilityDecision CreateHealDecision()
    {
        var decision = new UtilityDecision { DecisionName = "Heal Allies", Action = new HealAction { ActionName = "Healing" } };
        decision.Considerations.Add(CreateConsideration("HealNeedScore", new Vector2(0.0f, 0.0f), new Vector2(1.0f, 1.0f)));
        decision.Considerations.Add(CreateConsideration("CanHealAgain", new Vector2(0.0f, 0.0f), new Vector2(1.0f, 1.0f)));
        return decision;
    }

    private UtilityDecision CreateFireballDecision()
    {
        var decision = new UtilityDecision { DecisionName = "Cast Fireball", Action = new CastFireballAction { ActionName = "Casting Fireball" } };
        decision.Considerations.Add(CreateConsideration("MidRangeScore", new Vector2(0.0f, 0.3f), new Vector2(0.1f, 1.0f), new Vector2(1.0f, 0.0f)));
        return decision;
    }

    private UtilityDecision CreateChaseDecision()
    {
        var decision = new UtilityDecision { DecisionName = "Chase Target", Action = new MoveAction { ActionName = "Chasing" } };
        decision.Considerations.Add(CreateConsideration("MidRangeScore", new Vector2(0.0f, 0.0f), new Vector2(0.3f, 0.8f), new Vector2(1.0f, 1.0f)));
        decision.Considerations.Add(CreateConsideration("ThreatLevel", new Vector2(0.0f, 1.0f), new Vector2(1.0f, 0.3f)));
        return decision;
    }

    private UtilityDecision CreateWaitDecision()
    {
        var decision = new UtilityDecision { DecisionName = "Wait", Action = new WaitAction { ActionName = "Waiting" } };
        decision.Considerations.Add(CreateConsideration("FarRangeScore", new Vector2(0.0f, 0.0f), new Vector2(1.0f, 1.0f)));
        return decision;
    }

    private float CalculateHealNeedScore()
    {
        if (_host == null || _host.Self == null)
            return 0.0f;

        var allies = _host.Self.GetTree().GetNodesInGroup("enemies");
        float lowestHealthRatio = 1.0f;

        foreach (Node node in allies)
        {
            if (node is not Enemy enemy || enemy == _host.Self || enemy.IsDead || enemy.stats == null)
                continue;

            float distance = _host.Self.GlobalPosition.DistanceTo(enemy.GlobalPosition);
            if (distance > HealRadius)
                continue;

            float currentHealth = enemy.stats.GetStat(StatsID.CurrentHealth);
            float maxHealth = enemy.stats.GetStat(StatsID.MaxHealth);
            float healthRatio = maxHealth > 0f ? currentHealth / maxHealth : 1f;

            if (healthRatio < lowestHealthRatio)
            {
                lowestHealthRatio = healthRatio;
            }
        }

        if (lowestHealthRatio >= 0.7f)
            return 0.0f;

        return Mathf.Clamp(1.0f - lowestHealthRatio, 0.0f, 1.0f);
    }

    private static UtilityConsideration CreateConsideration(string blackboardKey, params Vector2[] points)
    {
        var curve = new Curve();
        foreach (Vector2 point in points)
        {
            curve.AddPoint(point);
        }

        return new UtilityConsideration
        {
            BlackboardKey = blackboardKey,
            ResponseCurve = new ResponseCurve { CurveData = curve }
        };
    }
}
