using Godot;

public partial class Inverter: BehaviourTree
{
    public BehaviourTree Child;

    public override NodeStatus Execute(double delta)
    {
        if (Child == null)
            return NodeStatus.Failure;

        NodeStatus status = Child.Execute(delta);

        return status switch
        {
            NodeStatus.Success => NodeStatus.Failure,
            NodeStatus.Failure => NodeStatus.Success,
            NodeStatus.Running => NodeStatus.Running,
            _ => NodeStatus.Failure
        };
    }
}