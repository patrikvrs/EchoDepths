using Godot;

public partial class AIControllerChooser : Node
{
    public enum AIMode
    {
        BehaviorTree,
        UtilityAI,
        Hybrid
    }

    [Export]
    private Label3D debugLabel;
    [Export] private AIMode mode = AIMode.BehaviorTree;
    [Export] private Node behaviorTreeControllerNode;
    [Export] private Node utilityAIControllerNode;
    [Export] private Node hybridAIControllerNode;

    private IAIHost _host;
    private IAIController _activeController;
    private Blackboard _blackboard;

    private double _timer;
    private const double TickRate = 0.1f; //10 times per second

    public override void _Ready()
    {
        _host = GetParent() as IAIHost;

        if (_host == null)
        {
            GD.PushError("AIControllerChooser parent must implement IAIHost.");
            return;
        }

        _blackboard = new Blackboard();
        SwitchMode(mode);
    }

    public override void _PhysicsProcess(double delta)
    {
        _timer += delta;
        while(_timer >= TickRate)
        {
            _timer -= TickRate;
            _activeController?.Tick(TickRate);
        }
    }

    public void SwitchMode(AIMode newMode)
    {
        _activeController?.Stop();

        mode = newMode;
        _activeController = ResolveControllerForMode(mode);

        if (_activeController == null)
        {
            GD.PushWarning($"AI controller is not set correctly for mode {mode}.");
            return;
        }

        _activeController.Setup(_host, _blackboard);
    }

    private IAIController ResolveControllerForMode(AIMode aiMode)
    {
        return aiMode switch
        {
            AIMode.BehaviorTree => ValidateController(behaviorTreeControllerNode, nameof(behaviorTreeControllerNode)),
            AIMode.UtilityAI => ValidateController(utilityAIControllerNode, nameof(utilityAIControllerNode)),
            AIMode.Hybrid => ValidateController(hybridAIControllerNode, nameof(hybridAIControllerNode)),
            _ => null
        };
    }

    private IAIController ValidateController(Node controllerNode, string fieldName)
    {
        if (controllerNode == null)
        {
            GD.PushWarning($"{fieldName} is not assigned on {Name}.");
            return null;
        }

        if (controllerNode is IAIController controller)
            return controller;

        GD.PushWarning($"{fieldName} must reference a node that implements IAIController.");

        return null;
    }
}