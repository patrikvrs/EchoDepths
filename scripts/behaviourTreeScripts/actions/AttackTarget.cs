using Godot;

public partial class AttackTarget : BehaviourTree
{
    public new Node3D Owner;
    public Blackboard BB;
    public float AttackDamage = 0f;
    public bool UseOwnerAttackDamage = true;

    public override NodeStatus Execute(double delta)
    {
        BB?.Set("LastActionName", "Attacking Target");

        if (Owner == null || BB == null)
            return NodeStatus.Failure;

        if (Owner is CharacterBody3D body)
            body.Velocity = Vector3.Zero;

        if (!BB.TryGet("Target", out Node3D target) || target == null)
            return NodeStatus.Failure;

        if (target is not IDamageable damageable)
            return NodeStatus.Failure;

        float damage = AttackDamage;
        if (UseOwnerAttackDamage && Owner is IAIHost host)
            damage = host.GetStat(StatsID.AttackDamage);

        damageable.TakeDamage(damage);
        return NodeStatus.Success;
    }
}