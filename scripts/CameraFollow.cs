using Godot;
using System;

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
}
