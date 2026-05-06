using Godot;

public partial class PlayerStats : CharacterBase
{
    protected override void InitStats()
    {
        SetStat(StatsID.MaxHealth, 100);
        SetStat(StatsID.CurrentHealth, 100);
        SetStat(StatsID.AttackDamage, 15);
        SetStat(StatsID.AttackRange, 2.0f);
        SetStat(StatsID.AttackSpeed, 1.2f);
        SetStat(StatsID.MovementSpeed, 4.0f);
        SetStat(StatsID.MaxStamina, 50);
        SetStat(StatsID.CurrentStamina, 50);
    }
}
