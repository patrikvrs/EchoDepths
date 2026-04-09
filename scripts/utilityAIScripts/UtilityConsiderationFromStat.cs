using Godot;

[GlobalClass]
public partial class UtilityConsiderationFromStat : Resource
{
    [Export]
    public ResponseCurve ResponseCurve { get; set; } = new ResponseCurve();
    [Export]
    public StatsID StatID { get; set; }
    [Export]
    public float Weight { get; set; } = 1.0f;
    [Export]
    public float MaxValue { get; set; } = 50f;

    public float Evaluate(IAIHost host)
    {
        if (host == null || MaxValue <= 0f)
        {
            GD.PrintErr("Host is null or MaxValue is invalid. Zero will be returned.");
            return 0.0f;
        }

        float normalizedValue = Mathf.Clamp(host.GetStat(StatID) / MaxValue, 0f, 1f);

        return ResponseCurve.Evaluate(normalizedValue);
    }
}