using Godot;

public partial class InGameHUD : Control
{
    private Label _healthInNumber;
    private ProgressBar _healthBar;

    private Label _staminaInNumber;
    private ProgressBar _staminaBar;
    private PlayerStats _playerStats;
    private Label _scoreLabel;

    [Export]
    private Player _player;

    public override void _Ready()
    {
        _healthInNumber = GetNode<Label>("HP_Label/HP_number");
        _healthBar = GetNode<ProgressBar>("HP_Label/HP_bar");
        _staminaInNumber = GetNode<Label>("Stamina_Label/Stamina_number");
        _staminaBar = GetNode<ProgressBar>("Stamina_Label/Stamina_bar");

        _playerStats = _player.GetNode<PlayerStats>("PlayerStats");
        _scoreLabel = GetNode<Label>("Score_Label/Score");

    }

    public override void _Process(double delta)
    {
        if (_playerStats != null)
        {
            _healthInNumber.Text = $"{(int)_playerStats.GetStat(StatsID.CurrentHealth)}/{(int)_playerStats.GetStat(StatsID.MaxHealth)}";
            _healthBar.Value = _playerStats.GetStat(StatsID.CurrentHealth);
            _staminaInNumber.Text = $"{(int)_playerStats.GetStat(StatsID.CurrentStamina)}/{(int)_playerStats.GetStat(StatsID.MaxStamina)}";
            _staminaBar.Value = _playerStats.GetStat(StatsID.CurrentStamina);
            if (_scoreLabel != null && _player != null)
            {
                _scoreLabel.Text = _player.Score.ToString();
            }
        }
    }
}