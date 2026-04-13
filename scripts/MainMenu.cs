using Godot;

public partial class MainMenu : Control
{
    [Export]
    private PackedScene _gameScene;
    [Export]
    private Button _startButton;
    [Export]
    private Button _quitButton;

    public override void _Ready()
    {
        _startButton.Pressed += OnStartButtonPressed;
        _quitButton.Pressed += OnQuitButtonPressed;
    }

    private void OnStartButtonPressed()
    {
        if (_gameScene != null)
        {
            GetTree().ChangeSceneToPacked(_gameScene);
        }
        else
        {
            GD.PrintErr("Game scene is not assigned.");
        }
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }

}