using Godot;
public partial class Wait : BehaviourTree
{
    public float WaitTime = 1.0f;
    private float _elapsedTime = 0.0f;

    public override NodeStatus Execute()
    {
        //TODO: Fix timing using delta time
        _elapsedTime += (float)GetPhysicsProcessDeltaTime();

        if (_elapsedTime >= WaitTime)
        {
            _elapsedTime = 0.0f;
            return NodeStatus.Success;
        }

        return NodeStatus.Running;
    }
}