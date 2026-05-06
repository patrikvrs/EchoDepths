public partial class EnemyStats : CharacterBase
{
    protected override void InitStats()
    {
        SetStat(StatsID.MaxHealth, 100);
        SetStat(StatsID.CurrentHealth, GetStat(StatsID.MaxHealth));
        SetStat(StatsID.AttackDamage, 10);
        SetStat(StatsID.AttackRange, 1.5f);
        SetStat(StatsID.AttackSpeed, 1.0f);
        SetStat(StatsID.MovementSpeed, 4.0f);
    }
}