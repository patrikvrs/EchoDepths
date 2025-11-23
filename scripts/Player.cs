using Godot;

public partial class Player : CharacterBody3D
{
	[Export]
	public PlayerStats playerStats;

	public float Speed => playerStats.GetStat("MovementSpeed");

	public const float JumpVelocity = 4.5f;

	[Export]
	public Camera3D Camera;

	public override void _PhysicsProcess(double delta)
	{
		Vector3 newVelocity = Velocity;

		if (!IsOnFloor())
		{
			newVelocity += GetGravity() * (float)delta;
		}

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			newVelocity.Y = JumpVelocity;
		}

		if (Input.IsActionJustPressed("attack"))
		{
			AttackMeelee();
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

			newVelocity.X = direction.X * Speed;
			newVelocity.Z = direction.Z * Speed;
		}

		if (newVelocity.X != 0 || newVelocity.Z != 0)
		{
			LookAt(GlobalPosition + new Vector3(newVelocity.X, 0, newVelocity.Z), Vector3.Up);
		}	

		Velocity = newVelocity;
		MoveAndSlide();
	}

	private void AttackMeelee()
	{
		//TODO: Create a hitbox in the cursor's direction thats checking for enemies hit.
		GD.Print("Player performs a melee attack!");
	}

	private void AttackRanged()
	{
		//TODO: Create a raycast from the player to the cursor's direction (to max range) and send an object flying to it. 
		GD.Print("Player performs a ranged attack!");
	}

	private void GetMousePositionInWorld()
    {
        
    }
}
