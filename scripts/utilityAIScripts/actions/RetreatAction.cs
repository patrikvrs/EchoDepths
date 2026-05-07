using Godot;

[GlobalClass]
public partial class RetreatAction : UtilityAction
{
    [Export]
    public float RetreatDistance = 5f;

    public override void Execute()
    {
        if (_host == null || _host.Self == null || _host.Target == null)
        {
            GD.PrintErr("Host, self, or target is null. Cannot retreat.");
            return;
        }

        if (_host.Self is not CharacterBody3D body)
        {
            return;
        }

        Vector3 directionFromTarget = (body.GlobalPosition - _host.Target.GlobalPosition).Normalized();
        Vector3 theoreticalRetreatPosition = body.GlobalPosition + directionFromTarget * RetreatDistance;

        Vector3 snappedRetreatPosition = theoreticalRetreatPosition;

        NavigationAgent3D navigationAgent = _host.NavigationAgent;
        if (navigationAgent == null)
        {
            GD.PrintErr("Host navigation agent is null. Cannot retreat.");
            return;
        }

        Rid navMap = navigationAgent.GetNavigationMap();
        if (navMap.IsValid)
        {
            snappedRetreatPosition = NavigationServer3D.MapGetClosestPoint(navMap, snappedRetreatPosition);
        }

        navigationAgent.TargetPosition = snappedRetreatPosition;

        Vector3 finalPathPosition = navigationAgent.GetFinalPosition();
        float distanceToWall = body.GlobalPosition.DistanceTo(finalPathPosition);

        if (distanceToWall < 0.5f)
        {
            body.Velocity = Vector3.Zero;
            if (_blackboard != null)
                _blackboard.Set("LastActionName", "Retreat Blocked / Cornered");
            return;
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

        if (_blackboard != null)
        {
            _blackboard.Set("LastActionName", "Retreating");
        }
    }
}