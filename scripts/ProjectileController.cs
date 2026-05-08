using Godot;

public partial class ProjectileController : Node3D
{
    public float Damage = 1f;
    public ulong ShooterId = 0;
    public float Lifetime = 5f;
    public Vector3 Velocity = Vector3.Zero;

    private float _age = 0f;

    private void PlayHitSound(Node3D projectileRoot)
    {
        if (projectileRoot == null)
            return;

        var audioPlayer = projectileRoot.GetNodeOrNull<AudioStreamPlayer3D>("AudioStreamPlayer");
        if (audioPlayer == null || audioPlayer.Stream == null)
            return;

        Node playbackParent = projectileRoot.GetTree()?.CurrentScene;
        if (playbackParent == null)
            playbackParent = projectileRoot.GetParent();

        if (playbackParent == null)
            return;

        audioPlayer.Reparent(playbackParent);
        audioPlayer.Finished += audioPlayer.QueueFree;
        audioPlayer.Play();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Velocity != Vector3.Zero && GetParent() is Node3D parentNode)
        {
            Vector3 from = parentNode.GlobalPosition;
            Vector3 to = from + Velocity * (float)delta;

            PhysicsRayQueryParameters3D ray = PhysicsRayQueryParameters3D.Create(from, to);
            ray.CollideWithBodies = true;
            ray.CollideWithAreas = false;

            var space = parentNode.GetWorld3D().DirectSpaceState;
            var hit = space.IntersectRay(ray);
            if (hit.Count > 0 && hit.ContainsKey("collider"))
            {
                Node collider = (Node)hit["collider"];

                if ((ulong)collider.GetInstanceId() != ShooterId)
                {
                    if (collider is IDamageable damageable)
                        damageable.TakeDamage(Damage);

                    PlayHitSound(parentNode);

                    GetParent()?.QueueFree();
                    QueueFree();
                    return;
                }
            }

            parentNode.GlobalPosition = to;
            if (Velocity.LengthSquared() > 0.0001f)
                parentNode.LookAt(parentNode.GlobalPosition + Velocity, Vector3.Up);
        }

        _age += (float)delta;

        if (_age >= Lifetime)
        {
            GetParent()?.QueueFree();
            QueueFree();
        }
    }
}
