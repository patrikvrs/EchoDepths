using Godot;
using System.Collections.Generic;

public partial class CharacterBase : Node
{
    private Dictionary<StatsID, float> _stats = new Dictionary<StatsID, float>();

    public override void _Ready()
    {
        InitStats();
    }

    protected virtual void InitStats()
    {
        GD.Print("Warning: CharacterBase InitStats not overridden in derived class. Using default stats.");

        _stats[StatsID.MaxHealth] = 100;
        _stats[StatsID.CurrentHealth] = _stats[StatsID.MaxHealth];
        _stats[StatsID.AttackDamage] = 10;
        _stats[StatsID.AttackRange] = 1.5f;
        _stats[StatsID.AttackSpeed] = 1.0f;
        _stats[StatsID.MovementSpeed] = 5.0f;
        _stats[StatsID.MaxStamina] = 50;
        _stats[StatsID.CurrentStamina] = _stats[StatsID.MaxStamina];
    }

    public float GetStat(StatsID statName)
    {
        return _stats.TryGetValue(statName, out float value) ? value : 0f;
    }

    public void SetStat(StatsID statName, float value)
    {
        if (!(statName == StatsID.CurrentHealth))
        {
            _stats[statName] = value;
        }
        else
        {
            _stats[StatsID.CurrentHealth] = Mathf.Clamp(value, 0, _stats[StatsID.MaxHealth]);
        }


    }

    public void ModifyStat(StatsID statName, float delta)
    {
        if (_stats.TryGetValue(statName, out float value))
        {
            _stats[statName] = Mathf.Max(value + delta, 0);

            if (statName == StatsID.MaxHealth || statName == StatsID.CurrentHealth)
            {
                _stats[StatsID.CurrentHealth] = Mathf.Clamp(_stats[StatsID.CurrentHealth], 0, _stats[StatsID.MaxHealth]);
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
