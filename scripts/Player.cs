using Godot;

public partial class Player : CharacterBody3D, IDamagable
{
    [Export]
    public PlayerStats playerStats;

    [Export]
    public PackedScene MeleeAttackArea;

    public float Speed => playerStats.GetStat("MovementSpeed");

    private const float JumpVelocity = 4.5f;
    private float Deceleration = 16f;

    [Export]
    public CameraFollow Camera;

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
            AttackMelee();
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
        else
        {
            float decel = Deceleration * (float)delta;
            newVelocity.X = Mathf.MoveToward(Velocity.X, 0, decel);
            newVelocity.Z = Mathf.MoveToward(Velocity.Z, 0, decel);
        }

        if (newVelocity.X != 0 || newVelocity.Z != 0)
        {
            LookAt(GlobalPosition + new Vector3(newVelocity.X, 0, newVelocity.Z), Vector3.Up);
        }	

        Velocity = newVelocity;
        MoveAndSlide();
    }

    public void ApplyDamage(float damage)
    {
        playerStats.ModifyStat("CurrentHealth", -damage);
        GD.Print($"Player took {damage} damage. Current Health: {playerStats.GetStat("CurrentHealth")}/{playerStats.GetStat("MaxHealth")}");
        if (playerStats.GetStat("CurrentHealth") <= 0)
        {
            GD.Print("Player has been defeated!");
        }
    }

    private void AttackMelee()
    {
        Vector3 mousePos = Camera.GetMousePositionInWorld();
        Vector3 attackDir = mousePos - GlobalPosition;
        attackDir.Y = 0;

        if (attackDir.Length() < 0.01f)
            attackDir = Transform.Basis.Z;

        attackDir = attackDir.Normalized();

        Area3D meleeAreaInstance = MeleeAttackArea.Instantiate<Area3D>();
        AddChild(meleeAreaInstance);

        float attackRange = playerStats.GetStat("AttackRange");
        meleeAreaInstance.Scale = new Vector3(1, meleeAreaInstance.Scale.Y, attackRange);

        Vector3 areaPos = GlobalPosition + attackDir * (attackRange / 2);
        meleeAreaInstance.GlobalPosition = areaPos;
        meleeAreaInstance.GlobalRotation = new Vector3(0, Mathf.Atan2(attackDir.X, attackDir.Z), 0);

        Timer attackDurationTimer = new Timer();
        attackDurationTimer.WaitTime = 0.2f;
        attackDurationTimer.OneShot = true;
        attackDurationTimer.Timeout += () =>
        {
            meleeAreaInstance.QueueFree();
            attackDurationTimer.QueueFree();
        };
        AddChild(attackDurationTimer);
        attackDurationTimer.Start();
    }
}