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
            if (child.Execute() == NodeStatus.Success)
                return NodeStatus.Success;
        }
        return NodeStatus.Failure;
    }
}