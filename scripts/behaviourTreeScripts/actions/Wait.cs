public partial class Wait : BehaviourTree
{
    public Blackboard BB;
    public float WaitTime = 1.0f;
    private float _elapsedTime = 0.0f;

    public override NodeStatus Execute(double delta)
    {
        BB?.Set("LastActionName", "Waiting");

        _elapsedTime += (float)delta;

        if (_elapsedTime >= WaitTime)
        {
            Reset();
            return NodeStatus.Success;
        }

        return NodeStatus.Running;
    }


    public override void Reset()
    {
        _elapsedTime = 0.0f;
    }
}