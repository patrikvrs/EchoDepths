using Godot;

[GlobalClass]
public partial class ResponseCurve : Resource
{
    [Export]
    public Curve CurveData { get; set; } = new Curve();

    public float Evaluate(float input)
    {
        if (CurveData == null)
        {
            GD.PrintErr("CurveData is missing. Zero will be returned.");
            return 0.0f;
        }

        return CurveData.SampleBaked(Mathf.Clamp(input, 0.0f, 1.0f));
    }
}