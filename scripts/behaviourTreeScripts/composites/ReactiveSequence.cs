using Godot;
using System.Collections.Generic;

public partial class ReactiveSequence : BehaviourTree
{
    protected List<BehaviourTree> children = new();

    public void AddChild(BehaviourTree child) => children.Add(child);

    public override NodeStatus Execute(double delta)
    {
        if (children.Count == 0)
            return NodeStatus.Success;

        foreach (var child in children)
        {
            var status = child.Execute(delta);
            if (status == NodeStatus.Failure) return NodeStatus.Failure;
            if (status == NodeStatus.Running) return NodeStatus.Running;
        }

        return NodeStatus.Success;
    }

    public override void Reset()
    {
        foreach (var child in children)
            child.Reset();
    }
}