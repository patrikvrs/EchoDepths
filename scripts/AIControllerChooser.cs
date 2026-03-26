using Godot;

public partial class AIControllerChooser : Node
{
    public enum AIMode
    {
        BehaviorTree,
        UtilityAI,
        Hybrid
    }

    [Export] private AIMode mode = AIMode.BehaviorTree;

    [Export] private NodePath behaviorTreeControllerPath;
    [Export] private NodePath utilityAIControllerPath;
    [Export] private NodePath hybridAIControllerPath;

    private IAIHost _host;
    private IAIController _activeController;
    private IBlackboard _blackboard;

    public override void _Ready()
    {
        _host = GetParent() as IAIHost;
        _blackboard = new Blackboard();
    }
    //TODO: Add logic to switch between controllers based on the selected mode
}