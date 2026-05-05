using Godot;

[GlobalClass]
public partial class ChaseAction : UtilityAction
{
    public override void Execute()
    {
        if (_host == null || _host.Target == null)
        {
            GD.PrintErr("Host or target is null. Cannot execute action.");
            return;
        }

        NavigationAgent3D navigationAgent = _host.NavigationAgent;
        if (navigationAgent == null)
        {
            GD.PrintErr("Host navigation agent is null. Cannot execute action.");
            return;
        }

        if (_host.Self is not CharacterBody3D body)
        {
            return;
        }

        navigationAgent.TargetPosition = _host.Target.GlobalPosition;
        Vector3 nextPosition = navigationAgent.GetNextPathPosition();
        Vector3 direction = nextPosition - body.GlobalPosition;
        direction.Y = 0f;

        if (direction.LengthSquared() > 0.0001f)
        {
            body.Velocity = direction.Normalized() * _host.GetStat(StatsID.MovementSpeed);
        }
        else
        {
            body.Velocity = Vector3.Zero;
        }

        _blackboard?.Set("LastActionName", "Chasing Target");
    }
}