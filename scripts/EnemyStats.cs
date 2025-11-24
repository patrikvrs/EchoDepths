using Godot;
using System;

public partial class EnemyStats : CharacterBase
{
    protected override void InitStats()
    {
        Level = 1;
        Stats["MaxHealth"] = 100 + (Level - 1) * 20;
        Stats["CurrentHealth"] = Stats["MaxHealth"];
        Stats["AttackDamage"] = 10 + (Level - 1) * 2;
        Stats["AttackRange"] = 1.5f;
        Stats["AttackSpeed"] = 1.0f;
        Stats["MovementSpeed"] = 4.0f;
    }
}