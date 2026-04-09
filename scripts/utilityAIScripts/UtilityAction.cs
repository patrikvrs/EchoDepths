using Godot;

[GlobalClass]
public partial class UtilityAction : Node
{
    [Export]
    public string ActionName { get; set; }
    [Export]
    public float UtilityScore { get; set; }

    private IAIHost _host;
    private Blackboard _blackboard;

    public void SetContext(IAIHost host, Blackboard blackboard)
    {
        _host = host;
        _blackboard = blackboard;
    }

    public virtual void Execute()
    {
        GD.Print($"Executing action: {ActionName}. Utility Score: {UtilityScore}");
    }
}