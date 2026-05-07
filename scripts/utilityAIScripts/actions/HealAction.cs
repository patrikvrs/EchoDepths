using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class HealAction : UtilityAction
{
    [Export]
    public float HealRadius = 15f;

    [Export]
    public float HealAmount = 15f;

    [Export]
    public float HealthThreshold = 0.6f;

    public override void Execute()
    {
        if (_host == null || _host.Self == null)
        {
            GD.PrintErr("Host or self is null. Cannot execute heal action.");
            return;
        }

        StopMovementAndClearNavigation();

        var alliesToHeal = FindLowHealthAllies();

        if (alliesToHeal.Count == 0)
        {
            GD.Print("HealAction: No allies to heal");
            return;
        }

        var mostInjuredAlly = alliesToHeal[0];
        if (mostInjuredAlly is Enemy allyEnemy && allyEnemy.stats != null)
        {
            allyEnemy.stats.ModifyStat(StatsID.CurrentHealth, HealAmount);

            // Play heal sound from the healer (host)
            if (_host?.Self is Enemy healer)
            {
                healer.PlayHealSound();
            }

            if (_blackboard != null)
            {
                _blackboard.Set("LastActionName", "Healing Allies");
                _blackboard.Set("HealJustExecuted", 1.0f);
            }

            GD.Print($"HealAction: {ActionName} - Healed ally for {HealAmount} HP");
        }
    }

    private List<Node3D> FindLowHealthAllies()
    {
        var allies = new List<Node3D>();
        var enemies = _host.Self.GetTree().GetNodesInGroup("enemies");

        foreach (Node node in enemies)
        {
            if (node is not Enemy enemy || enemy == _host.Self)
                continue;

            float distance = _host.Self.GlobalPosition.DistanceTo(enemy.GlobalPosition);
            if (distance > HealRadius)
                continue;

            if (enemy.stats != null)
            {
                float currentHealth = enemy.stats.GetStat(StatsID.CurrentHealth);
                float maxHealth = enemy.stats.GetStat(StatsID.MaxHealth);
                float healthRatio = maxHealth > 0 ? currentHealth / maxHealth : 1f;

                if (healthRatio < HealthThreshold && !enemy.IsDead)
                {
                    allies.Add(enemy);
                }
            }
        }

        allies.Sort((a, b) =>
        {
            if (a is Enemy enemyA && b is Enemy enemyB)
            {
                float healthA = enemyA.stats?.GetStat(StatsID.CurrentHealth) ?? 0;
                float healthB = enemyB.stats?.GetStat(StatsID.CurrentHealth) ?? 0;
                return healthA.CompareTo(healthB);
            }
            return 0;
        });

        return allies;
    }
}
