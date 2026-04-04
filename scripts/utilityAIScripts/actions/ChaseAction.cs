using Godot;

public partial class ChaseAction : UtilityAction
{
    private IAIHost _host;
    private NavigationAgent3D _navigationAgent;
    private Blackboard _blackboard;

    public ChaseAction(string actionName, float utilityScore, IAIHost host, Blackboard blackboard) 
        : base(actionName, utilityScore)
    {
        _host = host;
        _blackboard = blackboard;
        _navigationAgent = _host.NavigationAgent;
    }

    public override void Execute()
    {
        if(_host == null || _host.Target == null || _navigationAgent == null)
        {
            GD.PrintErr("Host, target, or navigation agent is null. Cannot execute action.");
            return;
        }

        Node3D target = _host.Target;
        if(_host.Self is CharacterBody3D body)
        {
            _navigationAgent.TargetPosition = target.GlobalPosition;
            Vector3 nextPosition = _navigationAgent.GetNextPathPosition();
            Vector3 direction = (nextPosition - body.GlobalPosition).Normalized();
            if (body.GlobalPosition.DistanceTo(nextPosition) > 0.1f)
            {
                body.Velocity = direction * _host.GetStat(CharacterBase.StatsID.MovementSpeed);
            }
            else
            {
                body.Velocity = Vector3.Zero;
            }
        }
    }
}