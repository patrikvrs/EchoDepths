using Godot;

public partial class BehaviourTree : Node
{
    public enum NodeStatus
    {
        Success,
        Failure,
        Running
    }

    public virtual NodeStatus Execute()
    {
        return NodeStatus.Failure;
    }

    public virtual void Reset()
    {
       
    }
}
