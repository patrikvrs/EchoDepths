using Godot;

public partial class ArrowController : Node3D
{
    public float Damage = 1f;
    public ulong ShooterId = 0;
    public float Lifetime = 5f;

    private float _age = 0f;

    public override void _PhysicsProcess(double delta)
    {
        _age += (float)delta;

        if (GetParent() is Node parent)
        {
            var detector = parent.GetNodeOrNull<Area3D>("Detector");
            if (detector != null)
            {
                var bodies = detector.GetOverlappingBodies();
                foreach (var obj in bodies)
                {
                    if (obj == null)
                        continue;

                    if (obj.GetInstanceId() == ShooterId)
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
