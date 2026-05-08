using Godot;

public partial class EndLevel : Area3D
{
    public override void _Ready()
    {
        base._Ready();
        Monitoring = true;
        CollisionMask = 2;
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        if (body is Player player)
        {
            var enemies = GetTree().GetNodesInGroup("enemies");
            if (enemies.Count == 0)
            {
                var endgameMenu = GetTree().CurrentScene?.FindChild("endgame_menu", true, false) as Control;
                if (endgameMenu != null)
                {
                    var titleLabel = endgameMenu.FindChild("Title", true, false) as Label;
                    if (titleLabel != null)
                    {
                        titleLabel.Text = "Level Completed!";
                    }

                    endgameMenu.Visible = true;
                    GetTree().Paused = true;
                }
            }
        }
    }
}