public partial class EnemyStats : CharacterBase
{
    protected override void InitStats()
    {
        Level = 1;
        SetStat(StatsID.MaxHealth, 100 + (Level - 1) * 20);
        SetStat(StatsID.CurrentHealth, GetStat(StatsID.MaxHealth));
        SetStat(StatsID.AttackDamage, 10 + (Level - 1) * 2);
        SetStat(StatsID.AttackRange, 1.5f);
        SetStat(StatsID.AttackSpeed, 1.0f);
        SetStat(StatsID.MovementSpeed, 4.0f);
    }
}