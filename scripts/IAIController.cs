using Godot;

public interface IAIController
{
    void Setup(IAIHost host, Blackboard blackboard);
    void Tick(double delta);
    void Stop();
}