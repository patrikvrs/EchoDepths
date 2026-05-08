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
    private HSlider _masterVolSlider;
    private Label _masterVolLabel;
    private AudioStreamPlayer _previewPlayer;

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
        _masterVolSlider = GetNode<HSlider>("OptionsPanel/MasterVolSlider");
        _masterVolLabel = GetNode<Label>("OptionsPanel/MasterVolSlider/MasterVolLabel");

        _startButton.Pressed += OnStartButtonPressed;
        _tutorialButton.Pressed += OnTutorialButtonPressed;
        _optionsButton.Pressed += OnOptionsButtonPressed;
        _quitButton.Pressed += OnQuitButtonPressed;
        _backButton.Pressed += OnOptionsButtonPressed;
        _fullScreenSwitch.Pressed += OnFullScreenSwitchToggled;
        if (_masterVolSlider != null)
        {
            var bus = AudioServer.GetBusIndex("Master");
            if (bus >= 0)
            {
                var db = AudioServer.GetBusVolumeDb(bus);
                _masterVolSlider.Value = Mathf.DbToLinear(db) * 100f;
            }
            _masterVolSlider.ValueChanged += OnMasterVolChanged;
            _masterVolSlider.GuiInput += OnMasterSliderGuiInput;
        }
        _previewPlayer = new AudioStreamPlayer();
        var stream = GD.Load<AudioStream>("res://assets/sounds/pickupCoin.wav");
        if (stream != null)
        {
            _previewPlayer.Stream = stream;
            AddChild(_previewPlayer);
        }
        if (_masterVolLabel != null)
        {
            _masterVolLabel.Text = ((int)_masterVolSlider.Value).ToString() + "%";
        }
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

    private void OnMasterVolChanged(double value)
    {
        var bus = AudioServer.GetBusIndex("Master");
        if (bus < 0)
        {
            GD.PrintErr("Master audio bus not found.");
            return;
        }
        float linear = (float)value / 100f;
        float db = Mathf.LinearToDb(linear);
        AudioServer.SetBusVolumeDb(bus, db);
        if (_masterVolLabel != null) _masterVolLabel.Text = ((int)value).ToString() + "%";
    }

    private void OnMasterSliderGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb && !mb.Pressed)
        {
            if (_previewPlayer != null) _previewPlayer.Play();
        }
        else if (@event is InputEventScreenTouch st && !st.Pressed)
        {
            if (_previewPlayer != null) _previewPlayer.Play();
        }
    }

}