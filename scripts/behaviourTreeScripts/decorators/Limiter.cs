using Godot;

public partial class Limiter : BehaviourTree
{
    public BehaviourTree Child;
    public int MaxExecutions { get; set; } = 1;
    private int _executionCount = 0;

    public Limiter(int maxExecutions = 1)
    {
        MaxExecutions = maxExecutions;
    }

    public override NodeStatus Execute()
    {
        if (Child == null)
            return NodeStatus.Failure;

        if (_executionCount >= MaxExecutions)
            return NodeStatus.Failure;

        NodeStatus status = Child.Execute();

        if (status == NodeStatus.Success || status == NodeStatus.Failure)
        {
            _executionCount++;
        }

        return status;
    }

    public override void Reset()
    {
        _executionCount = 0;
        Child?.Reset();
    }
}