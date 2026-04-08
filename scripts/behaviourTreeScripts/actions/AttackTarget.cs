using Godot;

public partial class AttackTarget : BehaviourTree
{
    public new Node3D Owner;
    public Blackboard BB;
    public float AttackDamage = 0f;

    public override NodeStatus Execute(double delta)
    {
        BB?.Set("LastActionName", "Attacking Target");

        if (BB == null || !BB.TryGet("Target", out Node3D target) || target is not IDamageable damageable)
            return NodeStatus.Failure;

        if (Owner is CharacterBody3D body) body.Velocity = Vector3.Zero;

        damageable.TakeDamage(AttackDamage);
        BB.Set("IsAttackOnCooldown", true);
        return NodeStatus.Success;
    }
}