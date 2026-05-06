using Godot;
public partial class EnemyStats : CharacterBase
{
    [Export]
    private float _maxHealth = 100f;
    [Export]
    private float _attackDamage = 10f;
    [Export]
    private float _attackRange = 1.5f;
    [Export]
    private float _attackSpeed = 1.0f;
    [Export]
    private float _movementSpeed = 4.0f;

    protected override void InitStats()
    {
        SetStat(StatsID.MaxHealth, _maxHealth);
        SetStat(StatsID.CurrentHealth, GetStat(StatsID.MaxHealth));
        SetStat(StatsID.AttackDamage, _attackDamage);
        SetStat(StatsID.AttackRange, _attackRange);
        SetStat(StatsID.AttackSpeed, _attackSpeed);
        SetStat(StatsID.MovementSpeed, _movementSpeed);
    }
}