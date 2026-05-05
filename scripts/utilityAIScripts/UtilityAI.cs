using Godot;

public partial class UtilityAI : Node, IAIController
{
	private const float ChaseDistance = 15f;
	private bool _isActive;
	private IAIHost _host;
	private Blackboard _blackboard;

	[Export]
	public Godot.Collections.Array<UtilityDecision> Decisions = new Godot.Collections.Array<UtilityDecision>();

	public void Setup(IAIHost host, Blackboard blackboard)
	{
		_isActive = true;
		_host = host;
		_blackboard = blackboard;
		Decisions = BuildDefaultDecisions();
	}

	public void Tick(double delta)
	{
		if (!_isActive || _host == null || _blackboard == null)
			return;

		UpdateTargetState();

		UtilityDecision bestDecision = null;
		float bestScore = -1f;
		foreach (UtilityDecision decision in Decisions)
		{
			if (decision == null || decision.Action == null)
				continue;

			float score = decision.Evaluate(_host, _blackboard);
			if (score > bestScore)
			{
				bestScore = score;
				bestDecision = decision;
			}
		}

		_blackboard.Set("UtilityScore", bestScore);
		if (bestDecision != null)
		{
			_blackboard.Set("BestDecisionName", bestDecision.DecisionName);
		}

		if (bestDecision != null && bestDecision.Action != null)
		{
			bestDecision.Action.SetContext(_host, _blackboard);
			bestDecision.Action.Execute();
		}
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
			_blackboard.Set("TargetInAttackRange", 0.0f);
			_blackboard.Set("TargetInChaseRange", 0.0f);
			return;
		}

		float targetDistance = _host.Self.GlobalPosition.DistanceTo(target.GlobalPosition);
		float attackRange = _host.GetStat(StatsID.AttackRange);

		_blackboard.Set("TargetDistance", targetDistance);
		_blackboard.Set("TargetInAttackRange", targetDistance <= attackRange ? 1.0f : 0.0f);
		_blackboard.Set("TargetInChaseRange", targetDistance <= ChaseDistance ? 1.0f : 0.0f);
	}

	private Godot.Collections.Array<UtilityDecision> BuildDefaultDecisions()
	{
		var decisions = new Godot.Collections.Array<UtilityDecision>
		{
			CreateAttackDecision(),
			CreateChaseDecision(),
			CreatePatrolDecision()
		};

		return decisions;
	}

	private static UtilityDecision CreateAttackDecision()
	{
		var decision = new UtilityDecision
		{
			DecisionName = "Attack Target",
			Action = new AttackAction
			{
				ActionName = "Attack Target"
			}
		};

		decision.Considerations.Add(CreateBinaryConsideration("HasTarget"));
		return decision;
	}

	private static UtilityDecision CreateChaseDecision()
	{
		var decision = new UtilityDecision
		{
			DecisionName = "Chase Target",
			Action = new ChaseAction
			{
				ActionName = "Chase Target"
			}
		};

		decision.Considerations.Add(CreateBinaryConsideration("HasTarget"));
		return decision;
	}

	private static UtilityDecision CreatePatrolDecision()
	{
		var decision = new UtilityDecision
		{
			DecisionName = "Patrol",
			Action = new PatrolAction
			{
				ActionName = "Patrol"
			}
		};

		decision.Considerations.Add(CreateBinaryConsideration("HasNoTarget"));
		return decision;
	}

	private static UtilityConsideration CreateBinaryConsideration(string blackboardKey)
	{
		return new UtilityConsideration
		{
			BlackboardKey = blackboardKey,
			Weight = 1.0f,
			ResponseCurve = CreateBinaryResponseCurve()
		};
	}

	private static ResponseCurve CreateBinaryResponseCurve()
	{
		Curve curve = new Curve();
		curve.AddPoint(new Vector2(0.0f, 0.0f));
		curve.AddPoint(new Vector2(1.0f, 1.0f));

		return new ResponseCurve
		{
			CurveData = curve
		};
	}
}