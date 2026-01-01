using Godot;
using System.Collections.Generic;

public partial class Selector : BehaviourTree
{
    protected List<BehaviourTree> children = new();
    private int _currentIndex = 0;

    public void AddChild(BehaviourTree child)
    {
        children.Add(child);
    }

    public override NodeStatus Execute()
    {
        if (children.Count == 0) return NodeStatus.Failure;

        while (_currentIndex < children.Count)
        {
            var status = children[_currentIndex].Execute();

            if (status == NodeStatus.Success)
            {
                _currentIndex = 0;
                return NodeStatus.Success;
            }

            if (status == NodeStatus.Running)
            {
                return NodeStatus.Running;
            }

            _currentIndex++;
        }

        _currentIndex = 0;
        return NodeStatus.Failure;
    }
}