using Godot;

public partial class SetNavigationTarget : BehaviourTree
{
    public new Node3D Owner;
    public Blackboard BB;
    public string TargetKey;
    public NavigationAgent3D NavAgent;

    public override NodeStatus Execute(double delta)
    {
        BB?.Set("LastActionName", "Setting Navigation Target");

        if (Owner == null || BB == null || string.IsNullOrEmpty(TargetKey) || NavAgent == null)
            return NodeStatus.Failure;

        BB.TryGet(TargetKey, out Node3D target);
        NavAgent.TargetPosition = target.GlobalPosition;

        return NodeStatus.Success;
    }
}