using Godot;

public interface IAIController
{
    void Setup(IAIHost host, IBlackboard blackboard);
    void Tick(double delta);
    void Stop();
}