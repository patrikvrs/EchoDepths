using Godot;

public interface IAIHost
{
    Node3D Self { get; }
    NavigationAgent3D NavigationAgent { get; }
    Node3D Target { get; }
    bool IsDead { get; }
    float GetStat(CharacterBase.StatsID stat);
}