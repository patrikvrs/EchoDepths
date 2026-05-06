using Godot;


public partial class Spike : Node3D
{
    private Area3D _area;
    public override void _Ready()
    {
        _area = GetNode<Area3D>("floor_mesh/spikes/Area3D");
    }

    public override void _PhysicsProcess(double delta)
    {
        foreach (Node3D body in _area.GetOverlappingBodies())
        {
            GD.Print("Spike damage applied to " + body.Name);
            if (body is IDamageable damageable)
            {
                damageable.TakeDamage(9999);
            }
        }
    }
}