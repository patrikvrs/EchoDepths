using Godot;

public partial class EnemyHealthBarUI : ProgressBar
{
    [Export]
    public Node3D TargetEnemy;

    [Export]
    public float HeightOffset = 4.0f;

    [Export]
    public float MaxVisibleDistance = 40.0f;

    public override void _Ready()
    {
        Visible = false;
    }

    public override void _Process(double delta)
    {
        if (TargetEnemy == null || !IsInstanceValid(TargetEnemy))
        {
            Visible = false;
            return;
        }

        Camera3D camera = GetViewport().GetCamera3D();
        if (camera == null) return;

        Vector3 targetPos3D = TargetEnemy.GlobalPosition + Vector3.Up * HeightOffset;

        float distanceToCamera = camera.GlobalPosition.DistanceTo(targetPos3D);
        if (distanceToCamera > MaxVisibleDistance || camera.IsPositionBehind(targetPos3D))
        {
            Visible = false;
            return;
        }

        if (camera.IsPositionBehind(targetPos3D))
        {
            Visible = false;
        }
        else
        {
            Visible = true;

            Vector2 screenPos = camera.UnprojectPosition(targetPos3D);
            GlobalPosition = screenPos - (Size / 2f);
        }
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        MaxValue = maxHealth;
        Value = currentHealth;
    }
}