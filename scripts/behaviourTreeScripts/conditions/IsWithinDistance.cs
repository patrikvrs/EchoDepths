using Godot;

public partial class IsWithinDistance : BehaviourTree
{
    public new Node3D Owner;
    public Blackboard BB;
    public string TargetKey;
    public float Distance;
    public bool UseOwnerStatDistance;
    public StatsID DistanceStat = StatsID.AttackRange;

    public override NodeStatus Execute(double delta)
    {
        if (Owner == null || BB == null || string.IsNullOrEmpty(TargetKey)) return NodeStatus.Failure;

        BB.TryGet<Node3D>(TargetKey, out var target);

        if (target == null) return NodeStatus.Failure;

        float currentDistance = Owner.GlobalPosition.DistanceTo(target.GlobalPosition);
        float thresholdDistance = Distance;

        if (UseOwnerStatDistance && Owner is IAIHost host)
            thresholdDistance = host.GetStat(DistanceStat);

        return currentDistance <= thresholdDistance ? NodeStatus.Success : NodeStatus.Failure;
    }
}