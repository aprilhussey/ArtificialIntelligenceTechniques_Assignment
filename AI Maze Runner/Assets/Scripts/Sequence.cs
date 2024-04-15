using System.Collections;
using System.Collections.Generic;

public class Sequence : Node
{
    protected List<Node> children = new List<Node>();
    public Sequence(List<Node> nodes)
    {
        children = nodes;
    }
    public override NodeState Evaluate()
    {
        bool isAnyChildRunnig = false;
        foreach (Node node in children)
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    isAnyChildRunnig = true;
                    break;
                case NodeState.SUCCESS:
                    break;
                case NodeState.FAILURE:
                    _nodeState = NodeState.FAILURE;
                    return _nodeState;
                default:
                    break;
            }
        }
        _nodeState = isAnyChildRunnig ? NodeState.RUNNING : NodeState.SUCCESS;
        return _nodeState;
    }
}