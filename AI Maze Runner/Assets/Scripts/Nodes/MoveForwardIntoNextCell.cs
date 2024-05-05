using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForwardIntoNextCell : Node
{
    private BehaviourTreeScript parent;
    private Transform transform;
    private float cellDistance;

    public MoveForwardIntoNextCell(BehaviourTreeScript parent, Transform transform, float cellDistance)
    {
        this.parent = parent;
        this.transform = transform;
        this.cellDistance = cellDistance;
    }

    private bool ObstacleFront()
    {
        return parent.ObstacleVisibleInDirection(transform.forward);
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Moving forward into next cell");
        if (transform.position == parent.targetCellPos)
        {
            return NodeState.SUCCESS;
        }
        else if (ObstacleFront())
        {
            return NodeState.FAILURE;
        }
        else
        {
            parent.movingToNextCell = true;
            parent.targetCellPos = transform.forward * cellDistance;
            return NodeState.RUNNING;
        }
    }
}