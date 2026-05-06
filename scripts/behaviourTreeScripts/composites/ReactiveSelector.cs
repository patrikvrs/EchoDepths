using System.Collections.Generic;

public partial class ReactiveSelector : BehaviourTree
{
    private readonly List<BehaviourTree> _children = new List<BehaviourTree>();

    public void AddChild(BehaviourTree child)
    {
        _children.Add(child);
    }

    public override void SetContext(IAIHost host, Blackboard blackboard)
    {
        base.SetContext(host, blackboard);

        foreach (var child in _children)
        {
            child.SetContext(host, blackboard);
        }
    }

    public override NodeStatus Execute(double delta)
    {
        if (_children.Count == 0) return NodeStatus.Failure;

        bool isActive = false;
        NodeStatus finalStatus = NodeStatus.Failure;

        foreach (var child in _children)
        {
            if (!isActive)
            {
                var status = child.Execute(delta);

                if (status == NodeStatus.Success || status == NodeStatus.Running)
                {
                    finalStatus = status;
                    isActive = true;

                }
            }
            else
            {
                child.Reset();
            }
        }

        return finalStatus;
    }

    public override void Reset()
    {
        foreach (var child in _children)
        {
            child.Reset();
        }
    }
}