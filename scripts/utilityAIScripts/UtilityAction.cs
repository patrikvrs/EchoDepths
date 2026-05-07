using Godot;

[GlobalClass]
public partial class UtilityAction : Resource
{
    [Export]
    public string ActionName { get; set; }
    [Export]
    public float UtilityScore { get; set; }

    protected IAIHost _host;
    protected Blackboard _blackboard;

    public void SetContext(IAIHost host, Blackboard blackboard)
    {
        _host = host;
        _blackboard = blackboard;
    }

    public virtual void Execute()
    {
        GD.Print($"Executing action: {ActionName}. Utility Score: {UtilityScore}");
    }

    protected void StopMovementAndClearNavigation()
    {
        if (_host?.Self is not CharacterBody3D body)
            return;

        body.Velocity = Vector3.Zero;

        var navigationAgent = _host.NavigationAgent ?? body.GetNodeOrNull<NavigationAgent3D>("NavigationAgent3D");
        if (navigationAgent != null)
        {
            navigationAgent.TargetPosition = body.GlobalPosition;
        }
    }
}