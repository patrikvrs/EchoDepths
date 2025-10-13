using Godot;
using System.Collections.Generic;

public partial class CharacterBase : Node
{
    public int Level { get; protected set; } = 1;
    protected Dictionary<string, float> Stats = new Dictionary<string, float>();

    public override void _Ready()
    {
        InitStats();
    }

    protected virtual void InitStats()
    {
        GD.Print("Warning: CharacterBase InitStats not overridden in derived class. Using default stats.");

        Stats["MaxHealth"] = 100 + (Level - 1) * 20;
        Stats["CurrentHealth"] = Stats["MaxHealth"];
        Stats["AttackDamage"] = 10 + (Level - 1) * 2;
        Stats["AttackRange"] = 1.5f;
        Stats["AttackSpeed"] = 1.0f;
        Stats["MovementSpeed"] = 5.0f;
    }

    public float GetStat(string statName)
    {
        return Stats.ContainsKey(statName) ? Stats[statName] : 0f;
    }

    public void SetStat(string statName, float value)
    {
        Stats[statName] = value;
    }
}
