using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoObstacleLeftFaceLeft : Node
{
    private BehaviourTreeScript parent;
    private Transform transform;

    public NoObstacleLeftFaceLeft(BehaviourTreeScript parent, Transform transform)
    {
        this.parent = parent;
        this.transform = transform;
    }

    private bool ObstacleLeft()
    {
        return parent.ObstacleVisibleInDirection(-transform.right);
    }

    public override NodeState Evaluate()
    {
        if (!ObstacleLeft() && !parent.movingToCell && !parent.movingToCoin)
        {
            // Rotate left
            transform.Rotate(0, -90, 0);
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }
}
