using Godot;

[GlobalClass]
public partial class UtilityDecision : Node
{
    [Export]
    public string DecisionName { get; set; }
    [Export]
    public Godot.Collections.Array<UtilityConsideration> Considerations { get; set; } = new();
    [Export]
    public Godot.Collections.Array<UtilityConsiderationFromStat> ConsiderationsFromStats { get; set; } = new();
    [Export]
    public UtilityAction Action { get; set; }

    public float Evaluate(IAIHost host, Blackboard blackboard)
    {
        if (blackboard == null || host == null)
        {
            GD.PrintErr("Blackboard or host is null. Zero will be returned.");
            return 0.0f;
        }

        if (Considerations == null || Considerations.Count == 0)
        {
            return 0.0f;
        }

        float weightedSum = 0.0f;
        float totalWeight = 0.0f;

        foreach (UtilityConsideration consideration in Considerations)
        {
            if (consideration == null)
                continue;

            float weight = Mathf.Max(consideration.Weight, 0.0f);
            float score = consideration.Evaluate(consideration.BlackboardKey, blackboard);

            weightedSum += score * weight;
            totalWeight += weight;
        }

        foreach (UtilityConsiderationFromStat consideration in ConsiderationsFromStats)
        {
            if (consideration == null)
                continue;

            float weight = Mathf.Max(consideration.Weight, 0.0f);
            float score = consideration.Evaluate(host);

            weightedSum += score * weight;
            totalWeight += weight;
        }

        if (totalWeight <= 0.0f)
        {
            return 0.0f;
        }

        return weightedSum / totalWeight;
    }
}