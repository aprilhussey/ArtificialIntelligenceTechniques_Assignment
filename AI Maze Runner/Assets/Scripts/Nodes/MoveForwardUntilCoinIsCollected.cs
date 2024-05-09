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
        if (CoinCollected() && Vector3.Distance(transform.position, parent.targetCoinPos) <= 0.1)
        {
            parent.movingToCoin = false;
            // Round out x, and z for current position handles issues with above logic and the inacuracies of floating point logic (Cheaper to not calculate the coinPos instead of ref this variable)
            transform.position = parent.targetCoinPos;
            parent.targetCoinPos = new Vector3(-1, -1, -1);
            return NodeState.SUCCESS;
        }
        else if (parent.movingToCoin)
        {
            // Performing task of going to next cell
            float step = parent.moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, parent.targetCoinPos, step);
            return NodeState.RUNNING;
        }
        else if (!parent.movingToCell)
        {
            // New task to go to next cell
            parent.movingToCoin = true;
            parent.targetCoinPos = parent.targetCoin.transform.position;
            return NodeState.RUNNING;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }
}
