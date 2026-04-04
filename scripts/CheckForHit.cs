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
                    damageable.TakeDamage(DamageAmount);
                }
            }
        }

        Monitoring = false;
        QueueFree();
    }
}