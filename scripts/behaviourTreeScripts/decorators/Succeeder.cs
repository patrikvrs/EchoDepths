using Godot;

public partial class Succeeder : BehaviourTree
{
    public BehaviourTree Child;

    public override NodeStatus Execute(double delta)
    {
        if (Child == null)
            return NodeStatus.Success;

        _ = Child.Execute(delta);

        return NodeStatus.Success;
    }
}