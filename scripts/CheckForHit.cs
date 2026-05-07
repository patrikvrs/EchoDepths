using Godot;

public partial class CheckForHit : Area3D
{
    public float DamageAmount { get; set; }

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
        if (enemy.Blackboard.TryGet("IsBlocking", out bool isBlocking) && isBlocking)
        {
            Vector3 attackDir = GlobalPosition - body.GlobalPosition;
            attackDir.Y = 0;

            Vector3 forward = -body.GlobalTransform.Basis.Z;
            forward.Y = 0;

            if (attackDir.LengthSquared() <= 0.0001f || forward.LengthSquared() <= 0.0001f)
                return false;

            attackDir = attackDir.Normalized();
            forward = forward.Normalized();

            if (attackDir.Dot(forward) < 0f)
                forward = -forward;

            float dot = attackDir.Dot(forward);
            return dot > 0.5f;
        }

        int hitCount = 0;
        enemy.Blackboard.TryGet("HitCount", out hitCount);
        enemy.Blackboard.Set("HitCount", hitCount + 1);

        return false;
    }
}