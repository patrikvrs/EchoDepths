using Godot;

public partial class HasTarget : BehaviourTree
{
    public new Node3D Owner;
    public IBlackboard BB;
    public string TargetKey;

    public override NodeStatus Execute(double delta)
    {
        if (Owner == null || BB == null || string.IsNullOrEmpty(TargetKey)) return NodeStatus.Failure;

        BB.TryGet<Node3D>(TargetKey, out var target);

        return target != null ? NodeStatus.Success : NodeStatus.Failure;
    }
}