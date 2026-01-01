using Godot;

public partial class Repeater : BehaviourTree
{
    public BehaviourTree Child;

    public override NodeStatus Execute()
    {
        if (Child == null)
            return NodeStatus.Failure;

        NodeStatus status = Child.Execute();

        if (status == NodeStatus.Success || status == NodeStatus.Failure)
        {
            Child.Reset();
            return NodeStatus.Running;
        }

        return status;
    }

    public override void Reset()
    {
        Child?.Reset();
    }
}