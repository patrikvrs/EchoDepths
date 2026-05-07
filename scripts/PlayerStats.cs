using Godot;

public partial class PlayerStats : CharacterBase
{
    [Export]
    private float _maxHealth = 100f;
    [Export]
    private float _attackDamage = 15f;
    [Export]
    private float _attackRange = 1.5f;
    [Export]
    private float _attackSpeed = 1.5f;
    [Export]
    private float _movementSpeed = 4.0f;
    [Export]
    private float _maxStamina = 50f;

    protected override void InitStats()
    {
        SetStat(StatsID.MaxHealth, _maxHealth);
        SetStat(StatsID.CurrentHealth, _maxHealth);
        SetStat(StatsID.AttackDamage, _attackDamage);
        SetStat(StatsID.AttackRange, _attackRange);
        SetStat(StatsID.AttackSpeed, _attackSpeed);
        SetStat(StatsID.MovementSpeed, _movementSpeed);
        SetStat(StatsID.MaxStamina, _maxStamina);
        SetStat(StatsID.CurrentStamina, _maxStamina);
    }
}
