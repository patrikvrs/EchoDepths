using Godot;

public partial class CheckForHit : Area3D
{
    public float DamageAmount { get; set; }
    public Vector3 AttackOrigin { get; set; }

    public async override void _Ready()
    {
        Monitoring = true;
        CollisionMask = 4;

        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        DealDamage();
    }

    private void DealDamage()
    {
        var bodies = GetOverlappingBodies();
        foreach (var obj in bodies)
        {
            if (obj is CharacterBody3D body)
            {
                GD.Print("Hit detected on: " + body.Name);
                if (body is IDamageable damageable)
                {
                    if (TryHandleFrontBlock(body))
                    {
                        GD.Print("Enemy blocked the hit from the front: " + body.Name);
                        continue;
                    }

                    damageable.TakeDamage(DamageAmount);
                }
            }
        }

        Monitoring = false;
        QueueFree();
    }

    private bool TryHandleFrontBlock(CharacterBody3D body)
    {
        if (body is not Enemy enemy || enemy.Blackboard == null)
            return false;

        if (!enemy.Blackboard.TryGet("IsBlocking", out bool isBlocking) || !isBlocking)
        {
            enemy.Blackboard.TryGet("HitCount", out int count);
            enemy.Blackboard.Set("HitCount", count + 1);
            return false;
        }

        Vector3 attackDir = AttackOrigin - body.GlobalPosition;
        attackDir.Y = 0;
        if (attackDir.LengthSquared() < 0.001f) return false;
        attackDir = attackDir.Normalized();

        Vector3 forward = enemy.LogicalForward;
        float dot = attackDir.Dot(forward);

        if (dot > 0.7f)
        {
            return true;
        }
        return false;
    }
}