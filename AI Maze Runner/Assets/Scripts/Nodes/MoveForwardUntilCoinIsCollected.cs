using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForwardUntilCoinIsCollected : Node
{
    private BehaviourTreeScript parent;
    private Transform transform;
    private float cellDistance;

    public MoveForwardUntilCoinIsCollected(BehaviourTreeScript parent, Transform transform, float cellDistance)
    {
        this.parent = parent;
        this.transform = transform;
        this.cellDistance = cellDistance;
    }
    
    private bool CoinCollected()
    {
        return parent.targetCoin == null;
    }

    public override NodeState Evaluate()
    {
        if (CoinCollected())
        {
            Debug.Log("Coin Collected!");
            parent.movingToTarget = false;
            parent.targetCellPos = new Vector3(-1, -1, -1);
            // Round out x, and z for current position handles issues with above logic and the inacuracies of floating point logic
            transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            return NodeState.SUCCESS;
        }
        else if (parent.movingToTarget)
        {
            Debug.Log("Moving to Target");
            // Performing task of going to next cell
            float step = parent.moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, parent.targetCellPos, step);
            return NodeState.RUNNING;
        }
        else
        {
            Debug.Log("Moving to coin");
            // New task to go to next cell
            parent.movingToTarget = true;
            parent.targetCellPos = parent.targetCoin.transform.position;
            return NodeState.RUNNING;
        }
    }
}
