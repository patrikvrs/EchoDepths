using Godot;
using System.Collections.Generic;

public partial class CharacterBase : Node
{
    private Dictionary<StatsID, float> Stats = new Dictionary<StatsID, float>();

    public override void _Ready()
    {
        InitStats();
    }

    protected virtual void InitStats()
    {
        GD.Print("Warning: CharacterBase InitStats not overridden in derived class. Using default stats.");

        Stats[StatsID.MaxHealth] = 100;
        Stats[StatsID.CurrentHealth] = Stats[StatsID.MaxHealth];
        Stats[StatsID.AttackDamage] = 10;
        Stats[StatsID.AttackRange] = 1.5f;
        Stats[StatsID.AttackSpeed] = 1.0f;
        Stats[StatsID.MovementSpeed] = 5.0f;
        Stats[StatsID.MaxStamina] = 50;
        Stats[StatsID.CurrentStamina] = Stats[StatsID.MaxStamina];
    }

    public float GetStat(StatsID statName)
    {
        return Stats.TryGetValue(statName, out float value) ? value : 0f;
    }

    public void SetStat(StatsID statName, float value)
    {
        if (!(statName == StatsID.CurrentHealth))
        {
            Stats[statName] = value;
        }
        else
        {
            Stats[StatsID.CurrentHealth] = Mathf.Clamp(value, 0, Stats[StatsID.MaxHealth]);
        }


    }

    public void ModifyStat(StatsID statName, float delta)
    {
        if (Stats.TryGetValue(statName, out float value))
        {
            Stats[statName] = Mathf.Max(value + delta, 0);

            if (statName == StatsID.MaxHealth || statName == StatsID.CurrentHealth)
            {
                Stats[StatsID.CurrentHealth] = Mathf.Clamp(Stats[StatsID.CurrentHealth], 0, Stats[StatsID.MaxHealth]);
            }
        }
    }

    public virtual void ApplyDamage(float damage)
    {
        if (damage <= 0) return;
        ModifyStat(StatsID.CurrentHealth, -damage);
        GD.Print($"Character took {damage} damage. Current Health: {GetStat(StatsID.CurrentHealth)}/{GetStat(StatsID.MaxHealth)}");
    }

    public bool IsDead()
    {
        return GetStat(StatsID.CurrentHealth) <= 0;
    }
}
