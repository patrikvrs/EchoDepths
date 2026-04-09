using Godot;

public partial class PatrolAction : UtilityAction
{
    private IAIHost _host;
    private Blackboard _blackboard;
    private NavigationAgent3D _navigationAgent;

    public PatrolAction(string actionName, float utilityScore, IAIHost host, Blackboard blackboard)
        : base(actionName, utilityScore)
    {
        _host = host;
        _blackboard = blackboard;

        if (_host != null)
        {
            _navigationAgent = _host.NavigationAgent;
        }
    }

    public override void Execute()
    {
        if (_host == null || _navigationAgent == null)
        {
            GD.PrintErr("Host or navigation agent is null. Cannot execute action.");
            return;
        }

        if (_navigationAgent.IsNavigationFinished())
        {
            Vector3 randomDirection = new Vector3(
                (float)(GD.Randf() * 2 - 1),
                0,
                (float)(GD.Randf() * 2 - 1)
            ).Normalized();

            float patrolRadius = 10f;
            Vector3 patrolPoint = _host.Self.GlobalPosition + randomDirection * patrolRadius;
            _navigationAgent.TargetPosition = patrolPoint;
        }

        if (_host.Self is CharacterBody3D body)
        {
            Vector3 nextPosition = _navigationAgent.GetNextPathPosition();
            Vector3 direction = (nextPosition - body.GlobalPosition).Normalized();
            if (body.GlobalPosition.DistanceTo(nextPosition) > 0.1f)
            {
                body.Velocity = direction * _host.GetStat(StatsID.MovementSpeed);
            }
            else
            {
                body.Velocity = Vector3.Zero;
            }
        }

        _blackboard?.Set("LastActionName", "Patrolling");
    }
}