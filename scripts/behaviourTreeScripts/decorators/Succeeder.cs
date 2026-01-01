using Godot;

public partial class Succeeder : BehaviourTree
{
    public BehaviourTree Child;

    public override NodeStatus Execute()
    {
        if (Child == null)
            return NodeStatus.Success;

        _ = Child.Execute();

        return NodeStatus.Success;
    }
}