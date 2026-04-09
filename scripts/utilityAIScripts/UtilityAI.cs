using Godot;

public partial class UtilityAI : Node, IAIController
{
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
	}

	public void Tick(double delta)
	{
		if (!_isActive || _host == null || _blackboard == null)
			return;

		UtilityDecision bestDecision = null;
		float bestScore = -1f;
		foreach (UtilityDecision decision in Decisions)
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
			bestDecision.Action.SetContext(_host, _blackboard);
			bestDecision.Action.Execute();
		}

	}

	public void Stop()
	{
		_isActive = false;
	}
}