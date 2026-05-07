using Godot;

[GlobalClass]
public partial class WaitAction : UtilityAction
{
    public override void Execute()
    {
        if (_host?.Self is not CharacterBody3D body)
        {
            return;
        }

        body.Velocity = Vector3.Zero;
        _blackboard?.Set("LastActionName", "Waiting");
    }
}
