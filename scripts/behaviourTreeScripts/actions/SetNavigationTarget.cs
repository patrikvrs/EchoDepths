using Godot;

public partial class SetNavigationTarget : BehaviourTree
{
    public string TargetKey;

    public override NodeStatus Execute(double delta)
    {
        _blackboard?.Set("LastActionName", "Setting Navigation Target");

        if (_host?.Self == null || _blackboard == null || string.IsNullOrEmpty(TargetKey) || _host.NavigationAgent == null)
            return NodeStatus.Failure;

        if (!_blackboard.TryGet(TargetKey, out Node3D target) || target == null)
            return NodeStatus.Failure;

        Vector3 desiredTarget = target.GlobalPosition;

        Rid navMap = _host.NavigationAgent.GetNavigationMap();
        if (navMap.IsValid)
        {
            desiredTarget = NavigationServer3D.MapGetClosestPoint(navMap, desiredTarget);
        }

        _host.NavigationAgent.TargetPosition = desiredTarget;

        return NodeStatus.Success;
    }
}