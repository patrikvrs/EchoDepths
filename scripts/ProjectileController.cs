using Godot;

public partial class ProjectileController : Node3D
{
    public float Damage = 1f;
    public ulong ShooterId = 0;
    public float Lifetime = 5f;
    public string DetectorName = "Detector";
    public Vector3 Velocity = Vector3.Zero;

    private float _age = 0f;

    public override void _PhysicsProcess(double delta)
    {
        // Move the projectile by scripted velocity to avoid physics impulses on targets
        if (Velocity != Vector3.Zero && GetParent() is Node3D parentNode)
        {
            parentNode.GlobalPosition += Velocity * (float)delta;
            // Orient the projectile to face the travel direction
            if (Velocity.LengthSquared() > 0.0001f)
                parentNode.LookAt(parentNode.GlobalPosition + Velocity, Vector3.Up);
        }

        _age += (float)delta;

        if (GetParent() is Node parent)
        {
            var detector = parent.GetNodeOrNull<Area3D>(DetectorName);
            if (detector != null)
            {
                var bodies = detector.GetOverlappingBodies();
                foreach (var obj in bodies)
                {
                    if (obj == null)
                        continue;

                    if ((ulong)obj.GetInstanceId() == ShooterId)
                        continue;

                    if (obj is IDamageable damageable)
                    {
                        damageable.TakeDamage(Damage);
                        parent.QueueFree();
                        QueueFree();
                        return;
                    }
                }
            }
        }

        if (_age >= Lifetime)
        {
            GetParent()?.QueueFree();
            QueueFree();
        }
    }
}
