using Godot;

public partial class CoinPickup : Area3D
{
    [Export]
    private float _scoreValue = 10f;

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
            player.AddScore((int)_scoreValue);
            SetDeferred("monitoring", false);
            Visible = false;

            _audioPlayer.Play();
            _audioPlayer.Finished += () => QueueFree();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        RotateY(2f * (float)delta);
    }
}