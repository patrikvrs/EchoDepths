using Godot;

public partial class Player : CharacterBody3D, IDamageable
{
    private const float SprintBonusSpeed = 4f;
    private const float StaminaRegenPerSecond = 10f;
    private const float StaminaDrainPerSecond = 15f;
    private const float MinMoveDirectionLength = 0.01f;

    [Export]
    public PackedScene MeleeAttackArea;
    [Export]
    public CameraFollow Camera;

    public float Speed => _playerStats.GetStat(StatsID.MovementSpeed);
    private float CurrentSpeed => Speed + (_isSprinting ? SprintBonusSpeed : 0f);

    private const float _jumpVelocity = 4.5f;
    private float _deceleration = 16f;
    private bool _isSprinting = false;
    private PlayerStats _playerStats;
    private Control _gameOverMenu;
    private Control _gameplayHud;
    private AnimationTree _animationTree;
    private AnimationNodeStateMachinePlayback _animationState;

    public override void _Ready()
    {
        _animationTree = GetNodeOrNull<AnimationTree>("AnimationTree");
        if (_animationTree != null)
        {
            _animationState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/StateMachine/playback");
        }
        _playerStats = GetNode<PlayerStats>("PlayerStats");
        _gameOverMenu = GetTree().CurrentScene?.FindChild("endgame_menu", true, false) as Control;
        _gameplayHud = GetTree().CurrentScene?.FindChild("gameplay_hud", true, false) as Control;
    }

    public override void _PhysicsProcess(double delta)
    {
        float deltaTime = (float)delta;
        Vector3 newVelocity = Velocity;

        if (!IsOnFloor())
        {
            newVelocity += GetGravity() * deltaTime;
        }

        if (Input.IsActionJustPressed("jump") && IsOnFloor())
        {
            newVelocity.Y = _jumpVelocity;
        }

        if (Input.IsActionJustPressed("attack"))
        {
            SetAnimationState("Player_Attack");
            AttackMelee();
        }

        Sprint(deltaTime);
        Movement(ref newVelocity, deltaTime);
        HandleFacing(newVelocity);

        Velocity = newVelocity;
        MoveAndSlide();
    }

    public void TakeDamage(float damage)
    {
        if (_playerStats == null)
        {
            GD.PrintErr("PlayerStats is null. Cannot take damage.");
            return;
        }

        _playerStats.ApplyDamage(damage);

        if (_playerStats.IsDead())
        {
            SetAnimationState("Player_Death_A");
            SetPhysicsProcess(false);

            Timer deathTimer = new Timer();
            deathTimer.WaitTime = 1.5f;
            deathTimer.OneShot = true;
            deathTimer.Timeout += OnDeathTimerTimeout;
            AddChild(deathTimer);
            deathTimer.Start();
        }
    }

    private void OnDeathTimerTimeout()
    {
        GetTree().Paused = true;

        if (_gameOverMenu != null && _gameplayHud != null)
        {
            _gameOverMenu.Show();
            _gameplayHud.Hide();
        }
    }

    private void Sprint(float deltaTime)
    {
        bool sprintHeld = Input.IsActionPressed("sprint");
        float currentStamina = _playerStats.GetStat(StatsID.CurrentStamina);

        _isSprinting = sprintHeld && currentStamina > 0;

        if (_isSprinting)
        {
            float newStamina = Mathf.Max(0, currentStamina - StaminaDrainPerSecond * deltaTime);
            _playerStats.SetStat(StatsID.CurrentStamina, newStamina);
            if (newStamina <= 0)
            {
                _isSprinting = false;
            }
        }
        else if (currentStamina < _playerStats.GetStat(StatsID.MaxStamina))
        {
            float newStamina = Mathf.Min(_playerStats.GetStat(StatsID.MaxStamina), currentStamina + StaminaRegenPerSecond * deltaTime);
            _playerStats.SetStat(StatsID.CurrentStamina, newStamina);
        }
    }

    private void Movement(ref Vector3 velocity, float deltaTime)
    {
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        if (inputDir != Vector2.Zero && Camera != null)
        {
            Vector3 direction = GetMoveDirection(inputDir);
            velocity.X = direction.X * CurrentSpeed;
            velocity.Z = direction.Z * CurrentSpeed;
            SetAnimationState(_isSprinting ? "Player_Running_A" : "Player_Walking_A");
            return;
        }

        float decel = _deceleration * deltaTime;
        velocity.X = Mathf.MoveToward(velocity.X, 0, decel);
        velocity.Z = Mathf.MoveToward(velocity.Z, 0, decel);
        SetAnimationState("Idle");
    }

    private Vector3 GetMoveDirection(Vector2 inputDir)
    {
        Vector3 camDir = Camera.GlobalTransform.Basis.Z;
        camDir.Y = 0;
        camDir = camDir.Normalized();

        Vector3 camRight = Camera.GlobalTransform.Basis.X;
        camRight.Y = 0;
        camRight = camRight.Normalized();

        return (camDir * inputDir.Y + camRight * inputDir.X).Normalized();
    }

    private void HandleFacing(Vector3 velocity)
    {
        if (velocity.X == 0 && velocity.Z == 0)
        {
            return;
        }

        if (velocity.LengthSquared() > MinMoveDirectionLength * MinMoveDirectionLength)
        {
            Vector3 target = GlobalPosition + new Vector3(velocity.X, 0, velocity.Z);
            if (target.IsEqualApprox(GlobalPosition))
                return;

            LookAt(target, Vector3.Up);
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

        if (attackDir.Length() < MinMoveDirectionLength)
            attackDir = Transform.Basis.Z;

        attackDir = attackDir.Normalized();

        CheckForHit meleeAreaInstance = MeleeAttackArea.Instantiate<CheckForHit>();
        meleeAreaInstance.DamageAmount = _playerStats.GetStat(StatsID.AttackDamage);
        AddChild(meleeAreaInstance);

        float attackRange = _playerStats.GetStat(StatsID.AttackRange);
        meleeAreaInstance.Scale = new Vector3(1, meleeAreaInstance.Scale.Y, attackRange);

        Vector3 areaPos = GlobalPosition + attackDir * (attackRange / 2);
        meleeAreaInstance.GlobalPosition = areaPos;
        meleeAreaInstance.GlobalRotation = new Vector3(0, Mathf.Atan2(attackDir.X, attackDir.Z), 0);
    }

    private void SetAnimationState(string state)
    {
        if (_animationState == null)
            return;

        if (string.IsNullOrEmpty(state))
            return;

        _animationState.Travel(state);
    }
}