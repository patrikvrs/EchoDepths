using Godot;

public partial class BehaviourTree : Node
{
    protected IAIHost _host;
    protected Blackboard _blackboard;

    public enum NodeStatus
    {
        Success,
        Failure,
        Running
    }

    public virtual NodeStatus Execute(double delta)
    {
        return NodeStatus.Failure;
    }

    public virtual void SetContext(IAIHost host, Blackboard blackboard)
    {
        _host = host;
        _blackboard = blackboard;
    }

    public virtual void Reset()
    {

    }
}