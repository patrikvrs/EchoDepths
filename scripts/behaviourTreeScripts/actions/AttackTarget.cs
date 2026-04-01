using Godot;

public partial class AttackTarget : BehaviourTree
{
    public new Node3D Owner;
    public Blackboard BB;
    public float AttackDamage = 0f;
    public override NodeStatus Execute(double delta)
    {
        BB?.Set("LastActionName", "Attacking Target");

        if (Owner == null || BB == null)
            return NodeStatus.Failure;

        if (!BB.TryGet("Target", out Node3D target) || target == null)
            return NodeStatus.Failure;

        if (target is not IDamageable damageable)
            return NodeStatus.Failure;

        damageable.TakeDamage(AttackDamage);
        return NodeStatus.Success;
    }
}