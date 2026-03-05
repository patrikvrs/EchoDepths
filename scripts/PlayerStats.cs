using Godot;

public partial class PlayerStats : CharacterBase
{
    protected override void InitStats()
    {
        Level = 2;
        SetStat(StatsID.MaxHealth, 120);
        SetStat(StatsID.CurrentHealth, 120);
        SetStat(StatsID.AttackDamage, 15);
        SetStat(StatsID.AttackRange, 2.0f);
        SetStat(StatsID.AttackSpeed, 1.2f);
        SetStat(StatsID.MovementSpeed, 6.0f);
        PrintStats();
    }

    public void PrintStats()
    {
        GD.Print("Player Stats:");
        GD.Print($"Level: {Level}");
        GD.Print($"Health: {GetStat(StatsID.CurrentHealth)}/{GetStat(StatsID.MaxHealth)}");
        GD.Print($"Attack Damage: {GetStat(StatsID.AttackDamage)}");
        GD.Print($"Attack Range: {GetStat(StatsID.AttackRange)}");
        GD.Print($"Attack Speed: {GetStat(StatsID.AttackSpeed)}");
        GD.Print($"Movement Speed: {GetStat(StatsID.MovementSpeed)}");
    }
}
