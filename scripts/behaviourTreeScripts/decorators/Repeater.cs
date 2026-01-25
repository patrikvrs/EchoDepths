using Godot;

public partial class Repeater : BehaviourTree
{
    public BehaviourTree Child;

    public override NodeStatus Execute(double delta)
    {
        if (Child == null)
            return NodeStatus.Failure;

        NodeStatus status = Child.Execute(delta);

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