using Godot;

public partial class IsWithinDistance : BehaviourTree
{
    public new Node3D Owner;
    public Node3D Target;
    public float Distance;

    public override NodeStatus Execute()
    {
        if (Owner == null || Target == null)
            return NodeStatus.Failure;

        float currentDistance = Owner.GlobalPosition.DistanceTo(Target.GlobalPosition);

        return currentDistance <= Distance ? NodeStatus.Success : NodeStatus.Failure;
    }
}