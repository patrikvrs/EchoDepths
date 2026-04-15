using Godot;

public partial class MainMenu : Control
{
    [Export]
    private PackedScene _gameScene;
    private Button _startButton;
    private Button _optionsButton;
    private Button _quitButton;
    private BoxContainer _mainButtonsContainer;
    private Button _backButton;
    private Panel _optionsPanel;

    public override void _Ready()
    {
        _startButton = GetNode<Button>("MainMenuButtons/StartButton");
        _optionsButton = GetNode<Button>("MainMenuButtons/OptionsButton");
        _quitButton = GetNode<Button>("MainMenuButtons/QuitButton");
        _backButton = GetNode<Button>("OptionsPanel/BackButton");
        _mainButtonsContainer = GetNode<BoxContainer>("MainMenuButtons");
        _optionsPanel = GetNode<Panel>("OptionsPanel");

        _startButton.Pressed += OnStartButtonPressed;
        _optionsButton.Pressed += OnOptionsButtonPressed;
        _quitButton.Pressed += OnQuitButtonPressed;
        _backButton.Pressed += OnOptionsButtonPressed;

        _optionsPanel.Visible = false;
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

    private void OnOptionsButtonPressed()
    {
        _optionsPanel.Visible = !_optionsPanel.Visible;
        _mainButtonsContainer.Visible = !_mainButtonsContainer.Visible;
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }

}