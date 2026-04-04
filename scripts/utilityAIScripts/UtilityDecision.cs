using System.Collections.Generic;
using Godot;

public partial class UtilityDecision : Node
{
    public string DecisionName { get; set; }
    public List<UtilityConsideration> Considerations { get; set; } = new List<UtilityConsideration>();
    public UtilityAction Action { get; set; }

    public UtilityDecision(string decisionName, UtilityAction action)
    {
        DecisionName = decisionName;
        Action = action;
    }

    public float Evaluate(Blackboard blackboard)
    {
        if (blackboard == null)
        {
            GD.PrintErr("Blackboard is null. Zero will be returned.");
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

        if (totalWeight <= 0.0f)
        {
            return 0.0f;
        }

        return weightedSum / totalWeight;
    }
}