using System.Diagnostics;
using Godot;

public partial class MainMenu : Control
{
    [Export]
    private PackedScene _gameScene;
    [Export]
    private PackedScene _tutorialScene;
    private Button _startButton;
    private Button _tutorialButton;
    private Button _optionsButton;
    private Button _quitButton;
    private BoxContainer _mainButtonsContainer;
    private Button _backButton;
    private Panel _optionsPanel;
    private CheckButton _fullScreenSwitch;

    public override void _Ready()
    {
        _startButton = GetNode<Button>("MainMenuButtons/StartButton");
        _tutorialButton = GetNode<Button>("MainMenuButtons/TutorialButton");
        _optionsButton = GetNode<Button>("MainMenuButtons/OptionsButton");
        _quitButton = GetNode<Button>("MainMenuButtons/QuitButton");
        _backButton = GetNode<Button>("OptionsPanel/BackButton");
        _mainButtonsContainer = GetNode<BoxContainer>("MainMenuButtons");
        _optionsPanel = GetNode<Panel>("OptionsPanel");
        _fullScreenSwitch = GetNode<CheckButton>("OptionsPanel/FullScreenSwitch");

        _startButton.Pressed += OnStartButtonPressed;
        _tutorialButton.Pressed += OnTutorialButtonPressed;
        _optionsButton.Pressed += OnOptionsButtonPressed;
        _quitButton.Pressed += OnQuitButtonPressed;
        _backButton.Pressed += OnOptionsButtonPressed;
        _fullScreenSwitch.Pressed += OnFullScreenSwitchToggled;
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

    private void OnTutorialButtonPressed()
    {
        if (_tutorialScene != null)
        {
            GetTree().ChangeSceneToPacked(_tutorialScene);
        }
        else
        {
            GD.PrintErr("Tutorial scene is not assigned.");
        }
    }

    private void OnOptionsButtonPressed()
    {
        _optionsPanel.Visible = !_optionsPanel.Visible;
        _mainButtonsContainer.Visible = !_mainButtonsContainer.Visible;
    }

    private void OnFullScreenSwitchToggled()
    {
        if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen) DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        else DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }

}