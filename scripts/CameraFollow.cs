using Godot;

public partial class CameraFollow : Camera3D
{
    [Export]
    public Node3D Target;

    [Export]
    public Vector3 Offset = new Vector3(10, 20, -10);

    public override void _PhysicsProcess(double delta)
    {
        if (Target != null)
        {
            GlobalPosition = GlobalPosition.Lerp(Target.GlobalPosition + Offset, 0.1f);
        }
    }

    public Vector3 GetMousePositionInWorld()
    {
        Vector2 mousePos2D = GetViewport().GetMousePosition();
        Vector3 rayOrigin = ProjectRayOrigin(mousePos2D);
        Vector3 rayNormal = ProjectRayNormal(mousePos2D);

        float targetHeight = Target != null ? Target.GlobalPosition.Y : 0f;
        Plane groundPlane = new Plane(Vector3.Up, targetHeight);

        Vector3? result = groundPlane.IntersectsRay(rayOrigin, rayNormal);

        if (result.HasValue)
        {
            return result.Value;
        }

        return Target != null ? Target.GlobalPosition : Vector3.Zero;
    }
}