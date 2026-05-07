using Godot;

public partial class HasLineOfSight : BehaviourTree
{
    public string TargetKey = "Target";

    public override NodeStatus Execute(double delta)
    {
        if (_host?.Self == null || _blackboard == null || string.IsNullOrEmpty(TargetKey))
            return NodeStatus.Failure;

        if (!_blackboard.TryGet(TargetKey, out Node3D target) || target == null)
            return NodeStatus.Failure;

        Vector3 from = _host.Self.GlobalPosition + Vector3.Up * 1.2f;
        Vector3 to = target.GlobalPosition + Vector3.Up * 1.2f;

        PhysicsRayQueryParameters3D ray = PhysicsRayQueryParameters3D.Create(from, to);
        ray.CollideWithAreas = false;
        ray.CollideWithBodies = true;
        ray.CollisionMask = 1 << 0;


        var space = _host.Self.GetWorld3D().DirectSpaceState;
        var result = space.IntersectRay(ray);

        if (result.Count == 0)
            return NodeStatus.Success;

        if (result.ContainsKey("collider"))
        {
            Node colliderObj = (Node)result["collider"];
            Node cur = colliderObj;
            while (cur != null)
            {
                if (cur.GetInstanceId() == target.GetInstanceId())
                    return NodeStatus.Success;
                cur = cur.GetParent();
            }
        }

        return NodeStatus.Failure;
    }
}
