using Godot;

public partial class PlayerStats : CharacterBase
{
    protected override void InitStats()
    {
        Level = 2;
        Stats["MaxHealth"] = 150 + (Level - 1) * 30;
        Stats["CurrentHealth"] = Stats["MaxHealth"];
        Stats["AttackDamage"] = 15 + (Level - 1) * 3;
        Stats["AttackRange"] = 4.0f;
        Stats["AttackSpeed"] = 1.2f;
        Stats["MovementSpeed"] = 6.0f;
        PrintStats();
    }

    public void PrintStats()
    {
        GD.Print("Player Stats:");
        GD.Print($"Level: {Level}");
        GD.Print($"Health: {GetStat("CurrentHealth")}/{GetStat("MaxHealth")}");
        GD.Print($"Attack Damage: {GetStat("AttackDamage")}");
        GD.Print($"Attack Range: {GetStat("AttackRange")}");
        GD.Print($"Attack Speed: {GetStat("AttackSpeed")}");
        GD.Print($"Movement Speed: {GetStat("MovementSpeed")}");
    }
}
