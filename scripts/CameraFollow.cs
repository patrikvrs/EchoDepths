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
            //Lerp to the target position for smooth camera movement
            GlobalPosition = GlobalPosition.Lerp(Target.GlobalPosition + Offset, 0.1f);
        }
    }

    public Vector3 GetMousePositionInWorld()
    {
        float rayCastLength = 1000f;
        Vector3 from = ProjectRayOrigin(GetViewport().GetMousePosition());
        Vector3 to = from + ProjectRayNormal(GetViewport().GetMousePosition()) * rayCastLength;

        PhysicsRayQueryParameters3D rayParams = PhysicsRayQueryParameters3D.Create(from, to);
        rayParams.CollideWithAreas = false;
        rayParams.CollideWithBodies = true;
        var spaceState = GetWorld3D().DirectSpaceState;
        var result = spaceState.IntersectRay(rayParams);

        if (result.Count > 0)
        {
            Vector3 position = (Vector3)result["position"];
            return position;
        }
        return Vector3.Zero;
    }
}
