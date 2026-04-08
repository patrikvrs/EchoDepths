using Godot;

public partial class HasTarget : BehaviourTree
{
    public new Node3D Owner;
    public Blackboard BB;
    public string TargetKey;

    public override NodeStatus Execute(double delta)
    {
        if (Owner == null || BB == null || string.IsNullOrEmpty(TargetKey)) return NodeStatus.Failure;

        return BB.TryGet(TargetKey, out Node3D target) && target != null ? NodeStatus.Success : NodeStatus.Failure;
    }
}