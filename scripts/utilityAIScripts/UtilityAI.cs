using Godot;
using System.Collections.Generic;

public partial class UtilityAI : Node, IAIController
{
	private bool _isActive;
	private IAIHost _host;
	private Blackboard _blackboard;
	private List<UtilityDecision> _decisions = new List<UtilityDecision>();
	public void Setup(IAIHost host, Blackboard blackboard)
	{
		_isActive = true;
		_host = host;
		_blackboard = blackboard;
	}

	public void Tick(double delta)
	{
		if (!_isActive || _host == null || _blackboard == null)
			return;

		UtilityDecision bestDecision = null;
		float bestScore = -1f;
		foreach (UtilityDecision decision in _decisions)
		{
			if (decision == null || decision.Action == null)
				continue;

			float score = decision.Evaluate(_blackboard);
			if (score > bestScore)
			{
				bestScore = score;
				bestDecision = decision;
			}

		}

		if (bestDecision != null && bestDecision.Action != null)
		{
			bestDecision.Action.Execute();
		}

	}

	public void Stop()
	{
		_isActive = false;
	}
}