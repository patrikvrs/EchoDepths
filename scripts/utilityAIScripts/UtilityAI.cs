using Godot;

public partial class UtilityAI : Node, IAIController
{
	private bool _isActive;

	public void Setup(IAIHost host, IBlackboard blackboard)
	{
		_isActive = true;
	}

	public void Tick(double delta)
	{
		if (!_isActive)
			return;
	}

	private void MakeDecision()
	{
		
	}

	public void Stop()
	{
		_isActive = false;
	}
}