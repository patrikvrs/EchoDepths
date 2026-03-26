using Godot;
public partial class ResponseCurve : Resource
{
    [Export]
    public Curve CurveData { get; set; } = new Curve();

    public float Evaluate(float input)
    {
        return CurveData.SampleBaked(input);
    }
}