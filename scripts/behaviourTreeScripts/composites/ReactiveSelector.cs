using System.Collections.Generic;
using Godot;

public partial class ReactiveSelector : BehaviourTree
{
    private readonly List<BehaviourTree> _children = new List<BehaviourTree>();

    public void AddChild(BehaviourTree child)
    {
        _children.Add(child);
    }

    public override NodeStatus Execute()
    {
        foreach (var child in _children)
        {
            var status = child.Execute();
            
            if (status == NodeStatus.Success || status == NodeStatus.Running)
            {
                return status;
            }
        }
        
        return NodeStatus.Failure;
    }
}