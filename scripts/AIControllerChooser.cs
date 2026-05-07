using Godot;

public partial class AIControllerChooser : Node
{
    public enum AIMode
    {
        BehaviorTree,
        UtilityAI,
    }

    [Export]
    private Label3D debugLabel;
    [Export] private AIMode mode = AIMode.BehaviorTree;
    [Export] private Node behaviorTreeControllerNode;
    [Export] private Node utilityAIControllerNode;


    private IAIHost _host;
    private IAIController _activeController;
    private Blackboard _blackboard;

    private double _timer;
    private const double TickRate = 0.1f;

    public override void _Ready()
    {
        _host = GetParent() as IAIHost;

        if (_host == null)
        {
            GD.PushError("AIControllerChooser parent must implement IAIHost.");
            return;
        }

        _blackboard = new Blackboard();
        if (_host is Enemy enemy)
        {
            enemy.SetBlackboard(_blackboard);
        }
        SwitchMode(mode);
    }

    public override void _PhysicsProcess(double delta)
    {
        _timer += delta;
        while (_timer >= TickRate)
        {
            _timer -= TickRate;
            _activeController?.Tick(TickRate);
        }

        if (debugLabel != null)
        {
            string btStatus = _blackboard.TryGet("BTStatus", out BehaviourTree.NodeStatus status)
                ? status.ToString()
                : "None";

            string utilityScore = _blackboard.TryGet("UtilityScore", out float score)
                ? score.ToString("F2")
                : "N/A";

            string bestDecision = _blackboard.TryGet("BestDecisionName", out string decision)
                ? decision
                : "None";

            string allDecisionScores = string.Empty;
            if (_blackboard.TryGet("DecisionScores", out string ds) && !string.IsNullOrEmpty(ds))
            {
                allDecisionScores = ds;
            }

            debugLabel.Text = $"Mode: {mode}\nActive Controller: {_activeController?.GetType().Name ?? "None"}\nAction: {(_blackboard.TryGet("LastActionName", out string lastAction) ? lastAction : "None")}\nBest Decision: {bestDecision}\n" +
                              (string.IsNullOrEmpty(allDecisionScores) ? $"BT Status: {btStatus}" : $"Decision Scores:\n{allDecisionScores}");
        }
    }

    public void StopController()
    {
        _activeController?.Stop();
        _activeController = null;
        SetPhysicsProcess(false);
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