using Godot;

public partial class PlayerMovement : CharacterBody3D
{
	[Export]
	public PlayerStats playerStats;

	public float Speed => playerStats.GetStat("MovementSpeed");

	public const float JumpVelocity = 4.5f;

	[Export]
	public Camera3D Camera;

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;


		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (inputDir != Vector2.Zero && Camera != null)
		{
			Vector3 camDir = Camera.GlobalTransform.Basis.Z;
			camDir.Y = 0;
			camDir = camDir.Normalized();

			Vector3 camRight = Camera.GlobalTransform.Basis.X;
			camRight.Y = 0;
			camRight = camRight.Normalized();

			Vector3 direction = (camDir * inputDir.Y + camRight * inputDir.X).Normalized();

			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
