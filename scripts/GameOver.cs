using Godot;

public partial class GameOver : Control
{
    [Export]
    private string _mainMenuScenePath = "res://scenes/main_menu.tscn";

    private Button _restartButton;
    private Button _quitButton;

    public override void _Ready()
    {
        _restartButton = GetNode<Button>("GameOverButtons/RestartButton");
        _restartButton.Pressed += OnRestartButtonPressed;
        _quitButton = GetNode<Button>("GameOverButtons/QuitButton");
        _quitButton.Pressed += OnQuitButtonPressed;

        ProcessMode = ProcessModeEnum.Always;
    }

    private void OnRestartButtonPressed()
    {
        GetTree().Paused = false;
        GetTree().ReloadCurrentScene();
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile(_mainMenuScenePath);
    }
}