using Godot;

public partial class IsWithinDistance : BehaviourTree
{
    public string TargetKey;
    public float Distance;

    public override NodeStatus Execute(double delta)
    {
        if (_host?.Self == null || _blackboard == null || string.IsNullOrEmpty(TargetKey)) return NodeStatus.Failure;

        if (!_blackboard.TryGet(TargetKey, out Node3D target) || target == null) return NodeStatus.Failure;

        float currentDistance = _host.Self.GlobalPosition.DistanceTo(target.GlobalPosition);
        return currentDistance <= Distance ? NodeStatus.Success : NodeStatus.Failure;
    }
}