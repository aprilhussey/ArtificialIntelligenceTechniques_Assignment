using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoObstacleRightFaceRight : Node
{
    private BehaviourTreeScript parent;
    private Transform transform;

    public NoObstacleRightFaceRight(BehaviourTreeScript parent, Transform transform)
    {
        this.parent = parent;
        this.transform = transform;
    }

    private bool ObstacleRight ()
    {
        return parent.ObstacleVisibleInDirection(transform.right);
    }

    public override NodeState Evaluate()
    {
        if (!ObstacleRight() && !parent.movingToCell && !parent.movingToCoin)
        {
            Debug.Log("Turning Right");
            // Rotate Right
            transform.Rotate(0, 90, 0);
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }
}
