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
        if (Vector3.Distance(transform.position, parent.targetCellPos) <= 0.01)
        {
            parent.movingToTarget = false;
            parent.targetCellPos = new Vector3(-1, -1, -1);
            // Round out x, and z for current position handles issues with above logic and the inacuracies of floating point logic
            transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            return NodeState.SUCCESS;
        }
        else if (ObstacleFront())
        {
            return NodeState.FAILURE;
        } 
        else if (parent.movingToTarget)
        {
            // Performing task of going to next cell
            float step = parent.moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, parent.targetCellPos, step);
            return NodeState.RUNNING;
        }
        else
        {
            Debug.Log("Moving forward 1 cell");
            // New task to go to next cell
            parent.movingToTarget = true;
            parent.targetCellPos = transform.position + transform.forward * cellDistance;
            return NodeState.RUNNING;
        }
    }
}