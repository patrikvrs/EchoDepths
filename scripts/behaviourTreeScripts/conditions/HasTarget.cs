using Godot;

public partial class HasTarget : BehaviourTree
{
    public string TargetKey;

    public override NodeStatus Execute(double delta)
    {
        if (_host?.Self == null || _blackboard == null || string.IsNullOrEmpty(TargetKey)) return NodeStatus.Failure;

        return _blackboard.TryGet(TargetKey, out Node3D target) && target != null ? NodeStatus.Success : NodeStatus.Failure;
    }
}