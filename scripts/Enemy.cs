using Godot;

public partial class Enemy : CharacterBody3D, IDamageable, IAIHost
{
    private enum EnemySound
    {
        Hurt,
        Death,
        Attack,
        Heal
    }

    [Export]
    public NavigationAgent3D agent;
    [Export]
    public Node3D target;
    [Export]
    public EnemyStats stats;
    [Export]
    public int ScoreValue = 0;
    [Export]
    public float RotationLerpSpeed = 10.0f;
    [Export]
    public string DeathAnimationState;
    [Export]
    public AudioStream HurtSound;
    [Export]
    public AudioStream DeathSound;
    [Export]
    public AudioStream AttackSound;
    [Export]
    public AudioStream HealSound;
    [Export]
    public PackedScene healthBarScene;
    public Blackboard Blackboard { get; private set; }
    public Node3D Self => this;
    public NavigationAgent3D NavigationAgent => agent;
    public Node3D Target => target;
    public Vector3 LogicalForward;
    private AIControllerChooser chooser;
    private EnemyHealthBarUI _healthBarUI;
    private AnimationNodeStateMachinePlayback _animationState;
    public bool IsDead => stats == null || stats.IsDead();
    private bool _deathStateApplied;

    public override void _Ready()
    {
        AddToGroup("enemies");

        chooser = GetNodeOrNull<AIControllerChooser>("AIChooser");
        if (chooser == null)
        {
            GD.PrintErr("Enemy is missing AIControllerChooser child node.");
        }

        AnimationTree animationTree = GetNodeOrNull<AnimationTree>("AnimationTree");
        if (animationTree != null)
        {
            _animationState = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/StateMachine/playback");
        }

        if (healthBarScene != null)
        {
            _healthBarUI = healthBarScene.Instantiate<EnemyHealthBarUI>();
            _healthBarUI.TargetEnemy = this;

            Node ingameHud = GetTree().CurrentScene?.FindChild("gameplay_hud", true, false);
            if (ingameHud != null)
            {
                ingameHud.AddChild(_healthBarUI);
                _healthBarUI.UpdateHealth(stats.GetStat(StatsID.CurrentHealth), stats.GetStat(StatsID.MaxHealth));

            }
        }


    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsDead)
        {
            ApplyDeathState();
            return;
        }

        float deltaTime = (float)delta;

        if (!IsOnFloor())
        {
            Velocity += GetGravity() * deltaTime;
            if (Position.Y < -50f)
            {
                stats?.ApplyDamage(9999);
                ApplyDeathState();
                chooser?.StopController();
                return;
            }
        }

        var planarVelocity = new Vector3(Velocity.X, 0f, Velocity.Z);
        if (planarVelocity.LengthSquared() > 0.0001f)
        {
            float desiredYaw = Mathf.Atan2(planarVelocity.X, planarVelocity.Z);
            float currentYaw = Rotation.Y;
            float smoothedYaw = Mathf.LerpAngle(currentYaw, desiredYaw, RotationLerpSpeed * (float)delta);
            Rotation = new Vector3(Rotation.X, smoothedYaw, Rotation.Z);
        }

        MoveAndSlide();
    }

    private void ApplyDeathState()
    {
        if (_deathStateApplied)
            return;

        _deathStateApplied = true;
        Velocity = Vector3.Zero;

        if (_animationState != null && !string.IsNullOrEmpty(DeathAnimationState))
        {
            _animationState.Travel(DeathAnimationState);
        }

        if (_healthBarUI != null)
        {
            _healthBarUI.QueueFree();
        }

        DisableCollision();
        chooser?.StopController();

        GetTree().CreateTimer(2.0).Timeout += QueueFree;
    }

    private void DisableCollision()
    {
        CollisionShape3D hitbox = GetNodeOrNull<CollisionShape3D>("CollisionShape3D");
        if (hitbox != null) hitbox.Disabled = true;
    }


    public float GetStat(StatsID stat)
    {
        return stats?.GetStat(stat) ?? 0f;
    }

    public void RefreshHealthBar()
    {
        if (_healthBarUI != null && stats != null)
        {
            _healthBarUI.UpdateHealth(stats.GetStat(StatsID.CurrentHealth), stats.GetStat(StatsID.MaxHealth));
        }
    }

    public void TakeDamage(float damage)
    {
        if (stats == null)
        {
            GD.PrintErr("Stats is null. Cannot take damage.");
            return;
        }

        stats.ApplyDamage(damage);

        RefreshHealthBar();

        if (!stats.IsDead())
        {
            PlaySound(EnemySound.Hurt);
        }

        if (stats.IsDead())
        {
            GD.Print("Enemy has been defeated!");
            PlaySound(EnemySound.Death);

            if (!_deathStateApplied)
            {
                if (ScoreValue > 0)
                {
                    var player = target as Player;
                    if (player != null)
                    {
                        player.AddScore(ScoreValue);
                    }
                }
            }

            ApplyDeathState();
            chooser?.StopController();
        }
    }

    public void PlayAttackSound()
    {
        PlaySound(EnemySound.Attack);
    }

    public void PlayHealSound()
    {
        PlaySound(EnemySound.Heal);
    }

    private void PlaySound(EnemySound soundType)
    {
        AudioStream stream = soundType switch
        {
            EnemySound.Hurt => HurtSound,
            EnemySound.Death => DeathSound,
            EnemySound.Attack => AttackSound,
            EnemySound.Heal => HealSound,
            _ => null
        };

        if (stream == null)
            return;

        var soundPlayer = new AudioStreamPlayer3D
        {
            Stream = stream
        };

        AddChild(soundPlayer);
        soundPlayer.GlobalPosition = GlobalPosition;
        soundPlayer.Finished += soundPlayer.QueueFree;
        soundPlayer.Play();
    }

    public void SetBlackboard(Blackboard blackboard)
    {
        Blackboard = blackboard;
    }

}