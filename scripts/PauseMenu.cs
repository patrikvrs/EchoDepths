using Godot;

public partial class PauseMenu : Control
{
    [Export]
    private string _mainMenuScenePath = "res://scenes/main_menu.tscn";

    private Button _resumeButton;
    private Button _restartButton;
    private Button _quitButton;

    public override void _Ready()
    {
        _resumeButton = GetNode<Button>("PauseButtons/ResumeButton");
        _resumeButton.Pressed += OnResumeButtonPressed;
        _restartButton = GetNode<Button>("PauseButtons/RestartButton");
        _restartButton.Pressed += OnRestartButtonPressed;
        _quitButton = GetNode<Button>("PauseButtons/QuitButton");
        _quitButton.Pressed += OnQuitButtonPressed;

        ProcessMode = ProcessModeEnum.Always;
    }

    private void OnResumeButtonPressed()
    {
        GetTree().Paused = false;
        Visible = false;
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