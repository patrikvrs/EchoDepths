using Godot;

public partial class UtilityDecision : Node
{
    public string DecisionName { get; set; }
    public ResponseCurve ResponseCurve { get; set; }

    public UtilityDecision(string decisionName, ResponseCurve responseCurve)
    {
        DecisionName = decisionName;
        ResponseCurve = responseCurve;
    }

    public float Evaluate(float input)
    {
        if (ResponseCurve == null)
        {
            GD.PrintErr("ResponseCurve is missing. Zero will be returned.");
            return 0.0f;
        }

        return ResponseCurve.Evaluate(input);
    }
}