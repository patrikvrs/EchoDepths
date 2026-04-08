using Godot;

public partial class Player : CharacterBody3D, IDamageable
{
    [Export]
    public PlayerStats playerStats;

    [Export]
    public PackedScene MeleeAttackArea;

    public float Speed => playerStats.GetStat(StatsID.MovementSpeed);

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

    public void TakeDamage(float damage)
    {
        if (playerStats == null)
        {
            GD.PrintErr("PlayerStats is null. Cannot take damage.");
            return;
        }

        playerStats.ApplyDamage(damage);
        if (playerStats.IsDead())
        {
            GD.Print("Player has been defeated!"); //For debugging purposes, replace with death handling logic later
        }
    }

    private void AttackMelee()
    {
        if (Camera == null)
        {
            GD.PushWarning("Player Camera is not assigned. Melee attack skipped.");
            return;
        }

        if (MeleeAttackArea == null)
        {
            GD.PushWarning("Player MeleeAttackArea is not assigned. Melee attack skipped.");
            return;
        }

        Vector3 mousePos = Camera.GetMousePositionInWorld();
        Vector3 attackDir = mousePos - GlobalPosition;
        attackDir.Y = 0;

        if (attackDir.Length() < 0.01f)
            attackDir = Transform.Basis.Z;

        attackDir = attackDir.Normalized();

        CheckForHit meleeAreaInstance = MeleeAttackArea.Instantiate<CheckForHit>();
        meleeAreaInstance.DamageAmount = playerStats.GetStat(StatsID.AttackDamage);
        AddChild(meleeAreaInstance);

        float attackRange = playerStats.GetStat(StatsID.AttackRange);
        meleeAreaInstance.Scale = new Vector3(1, meleeAreaInstance.Scale.Y, attackRange);

        Vector3 areaPos = GlobalPosition + attackDir * (attackRange / 2);
        meleeAreaInstance.GlobalPosition = areaPos;
        meleeAreaInstance.GlobalRotation = new Vector3(0, Mathf.Atan2(attackDir.X, attackDir.Z), 0);
    }
}