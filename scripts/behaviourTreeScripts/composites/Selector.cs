using Godot;
using System.Collections.Generic;

public partial class Selector : BehaviourTree
{
    protected List<BehaviourTree> children = new();

    public void AddChild(BehaviourTree child)
    {
        children.Add(child);
    }

    public override NodeStatus Execute()
    {
        foreach (var child in children)
        {
            var result = child.Execute();
            if (result == NodeStatus.Success)
                return NodeStatus.Success;
            if (result == NodeStatus.Running)
                return NodeStatus.Running;
        }
        return NodeStatus.Failure;
    }
}