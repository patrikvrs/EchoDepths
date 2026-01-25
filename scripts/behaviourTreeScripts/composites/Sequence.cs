using Godot;
using System.Collections.Generic;

public partial class Sequence : BehaviourTree
{
    protected List<BehaviourTree> children = new();
    private int _currentIndex = 0;

    public void AddChild(BehaviourTree child)
    {
        children.Add(child);
    }

    public override NodeStatus Execute(double delta)
    {
        if (children.Count == 0)
            return NodeStatus.Success;

        while (_currentIndex < children.Count)
        {
            var status = children[_currentIndex].Execute(delta);

            if (status == NodeStatus.Success)
            {
                _currentIndex++; 
                continue;
            }

            if (status == NodeStatus.Running)
            {
                return NodeStatus.Running; 
            }

            _currentIndex = 0;
            return NodeStatus.Failure;
        }

        _currentIndex = 0;
        return NodeStatus.Success;
    }
    
    public override void Reset()
    {
        _currentIndex = 0;
        foreach (var child in children)
        {
            child.Reset();
        }
    }
}