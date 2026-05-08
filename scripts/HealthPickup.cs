using Godot;

public partial class HealthPickup : Area3D
{
    [Export]
    private float _healAmount = 50f;

    private AudioStreamPlayer _audioPlayer;

    public override void _Ready()
    {
        base._Ready();
        Monitoring = true;
        CollisionMask = 2;
        BodyEntered += OnBodyEntered;
        _audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
    }

    private void OnBodyEntered(Node body)
    {
        if (body is Player player)
        {
            PlayerStats stats = player.GetNode<PlayerStats>("PlayerStats");
            if (stats != null)
            {
                stats.ModifyStat(StatsID.CurrentHealth, _healAmount);
                SetDeferred("monitoring", false);
                Visible = false;

                _audioPlayer.Play();
            }

            _audioPlayer.Finished += () => QueueFree();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        RotateY(2f * (float)delta);
    }
}