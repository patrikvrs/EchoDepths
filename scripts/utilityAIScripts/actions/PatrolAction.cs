using Godot;

[GlobalClass]
public partial class PatrolAction : UtilityAction
{
    public override void Execute()
    {
        if (_host == null)
        {
            GD.PrintErr("Host is null. Cannot execute action.");
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

        if (navigationAgent.IsNavigationFinished())
        {
            Vector3 randomDirection = new Vector3(
                (float)(GD.Randf() * 2 - 1),
                0,
                (float)(GD.Randf() * 2 - 1)
            ).Normalized();

            float patrolRadius = 10f;
            Vector3 patrolPoint = _host.Self.GlobalPosition + randomDirection * patrolRadius;
            navigationAgent.TargetPosition = patrolPoint;
        }

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

        _blackboard?.Set("LastActionName", "Patrolling");
    }
}