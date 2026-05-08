using Godot;

[GlobalClass]
public partial class CastFireballAction : UtilityAction
{
    [Export]
    public string FireballScenePath = "res://scenes/fireball_projectile.tscn";

    [Export]
    public float FireballSpeed = 15f;

    [Export]
    public float FireballDamage = 20f;

    [Export]
    public float FireballLifetime = 3f;

    [Export]
    public float SpawnForwardOffset = 1.5f;

    [Export]
    private float _aimHeightOffset = 1.2f;

    [Export]
    private float _targetAimHeightOffset = 1.2f;

    public override void Execute()
    {
        if (_host == null || _host.Self == null || _host.Target == null)
        {
            GD.PrintErr("Host, self, or target is null. Cannot cast fireball.");
            return;
        }

        StopMovementAndClearNavigation();

        if (_blackboard == null)
        {
            return;
        }

        if (!_blackboard.TryGet("CanAttackAgain", out float canAttack) || canAttack < 1.0f)
        {
            return;
        }

        var fireballScene = GD.Load<PackedScene>(FireballScenePath);
        if (fireballScene == null)
        {
            GD.PrintErr($"Fireball scene not found at {FireballScenePath}. Creating placeholder projectile.");
            return;
        }

        var fireball = fireballScene.Instantiate<Node3D>();
        if (fireball == null)
        {
            GD.PrintErr("Failed to instantiate fireball.");
            return;
        }
        Node parent = _host.Self.GetParent();
        if (parent == null)
        {
            GD.PrintErr("Host has no parent. Cannot spawn fireball.");
            return;
        }

        var spawnPosition = _host.Self.GlobalPosition + _host.Self.GlobalTransform.Basis.Z * SpawnForwardOffset + _host.Self.GlobalTransform.Basis.Y * _aimHeightOffset;
        var aimPoint = _host.Target.GlobalPosition + Vector3.Up * _targetAimHeightOffset;
        var directionToTarget = (aimPoint - spawnPosition).Normalized();

        parent.AddChild(fireball);
        fireball.GlobalPosition = spawnPosition;

        var projectileController = new ProjectileController
        {
            Damage = FireballDamage,
            ShooterId = _host.Self.GetInstanceId(),
            Lifetime = FireballLifetime,
            Velocity = directionToTarget * FireballSpeed
        };
        fireball.AddChild(projectileController);

        _blackboard.Set("LastActionName", "Casting Fireball");
        _blackboard.Set("AttackJustExecuted", 1.0f);

        GD.Print($"CastFireballAction: {ActionName} - Cast fireball towards target");
    }
}
