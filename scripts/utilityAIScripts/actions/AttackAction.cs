using Godot;

[GlobalClass]
public partial class AttackAction : UtilityAction
{
    private IAIHost _host;
    private Blackboard _blackboard;

    public override void Execute()
    {
        if (_host == null || _host.Target == null)
        {
            GD.PrintErr("Host or target is null. Cannot execute action.");
            return;
        }

        Node3D target = _host.Target;
        if (target is not IDamageable damageable)
        {
            GD.PrintErr("Target is not damageable.");
            return;
        }

        if (_host.Self is CharacterBody3D body)
        {
            body.Velocity = Vector3.Zero;
        }

        float attackDamage = _host.GetStat(StatsID.AttackDamage);
        damageable.TakeDamage(attackDamage);

        if (_blackboard != null)
        {
            _blackboard.Set("LastActionName", "Attacking Target");
        }

        GD.Print($"AttackAction: {ActionName} - Dealt {attackDamage} damage to target");
    }
}