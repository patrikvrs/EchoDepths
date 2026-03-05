using Godot;
using System.Collections.Generic;

public partial class CharacterBase : Node
{
    public int Level { get; protected set; } = 1;

    public enum StatsID
    {
        MaxHealth,
        CurrentHealth,
        AttackDamage,
        AttackRange,
        AttackSpeed,
        MovementSpeed
    }

    private Dictionary<StatsID, float> Stats = new Dictionary<StatsID, float>();

    public override void _Ready()
    {
        InitStats();
    }

    protected virtual void InitStats()
    {
        GD.Print("Warning: CharacterBase InitStats not overridden in derived class. Using default stats.");

        Stats[StatsID.MaxHealth] = 100 + (Level - 1) * 20;
        Stats[StatsID.CurrentHealth] = Stats[StatsID.MaxHealth];
        Stats[StatsID.AttackDamage] = 10 + (Level - 1) * 2;
        Stats[StatsID.AttackRange] = 1.5f;
        Stats[StatsID.AttackSpeed] = 1.0f;
        Stats[StatsID.MovementSpeed] = 5.0f;
    }

    public float GetStat(StatsID statName)
    {
        return Stats.TryGetValue(statName, out float value) ? value : 0f;
    }

    public void SetStat(StatsID statName, float value)
    {
        Stats[statName] = value;
    }

    public void ModifyStat(StatsID statName, float delta)
    {
        if (Stats.TryGetValue(statName, out float value))
        {
            Stats[statName] = value + delta;
        }
    }
}
