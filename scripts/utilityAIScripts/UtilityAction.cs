using Godot;

public partial class UtilityAction : Node
{
    public string ActionName { get; set; }
    public float UtilityScore { get; set; }

    public UtilityAction(string actionName, float utilityScore)
    {
        ActionName = actionName;
        UtilityScore = utilityScore;
    }

    public virtual void Execute()
    {
        GD.Print($"Executing action: {ActionName}. Utility Score: {UtilityScore}");
    }
}