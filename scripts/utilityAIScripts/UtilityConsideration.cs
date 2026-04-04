using Godot;

public partial class UtilityConsideration : Resource
{
    [Export]
    public ResponseCurve ResponseCurve { get; set; } = new ResponseCurve();
    [Export]
    public string BlackboardKey { get; set; } = string.Empty;
    [Export]
    public float Weight { get; set; } = 1.0f;

    public float Evaluate(string blackboardKey, Blackboard blackboard)
    {
        if (blackboard.TryGet(blackboardKey, out float value))
        {
            return ResponseCurve.Evaluate(value);
        }

        GD.PrintErr($"Blackboard key '{blackboardKey}' not found or not a float. Zero will be returned.");
        return 0.0f;
    }
}