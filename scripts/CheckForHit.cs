using Godot;
using Godot.Collections;

public partial class CheckForHit : Area3D
{
    public override void _Ready()
    {
        Monitoring = true;
        CollisionMask = 4;
        CallDeferred(nameof(DealDamage));
    }

    private async void DealDamage() // Await one physics frame to ensure overlaps are detected
    {
        await ToSignal(GetTree(), "physics_frame");

        var bodies = GetOverlappingBodies();
        foreach (var obj in bodies)
        {
            if (obj is CharacterBody3D body)
            {
                GD.Print("Hit detected on: " + body.Name);
                if (body is IDamagable damagable)
                {
                    float damageAmount = 10f;
                    damagable.ApplyDamage(damageAmount);
                }
            }
        }
    }
}