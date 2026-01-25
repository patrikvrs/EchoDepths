using Godot;

public partial class IsWithinDistance : BehaviourTree
{
    public new Node3D Owner;
     public IBlackboard BB;
    public string TargetKey;
    public float Distance;

    public override NodeStatus Execute(double delta)
    {
        if (Owner == null || BB == null || string.IsNullOrEmpty(TargetKey)) return NodeStatus.Failure;

        BB.TryGet<Node3D>(TargetKey, out var target);

        float currentDistance = Owner.GlobalPosition.DistanceTo(target.GlobalPosition);

        return currentDistance <= Distance ? NodeStatus.Success : NodeStatus.Failure;
    }
}   