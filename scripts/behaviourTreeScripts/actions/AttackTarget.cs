using Godot;

public partial class AttackTarget : BehaviourTree
{
    public new Node3D Owner;
    public IBlackboard BB;
    public float AttackDamage = 0f;
    public override NodeStatus Execute(double delta)
    {
        BB?.Set("LastActionName", "Attacking Target");

        if (Owner == null || BB == null)
            return NodeStatus.Failure;

        if (!BB.TryGet("Target", out Node3D target) || target == null)
            return NodeStatus.Failure;

        if (target is not IDamagable damageable)
            return NodeStatus.Failure;

        damageable.ApplyDamage(AttackDamage);
        return NodeStatus.Success;
    }
}