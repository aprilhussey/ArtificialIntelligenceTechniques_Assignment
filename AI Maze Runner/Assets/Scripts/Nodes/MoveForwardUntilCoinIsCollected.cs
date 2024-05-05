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
        Debug.Log("Collecting coin");
        if (CoinCollected())
        {
            return NodeState.SUCCESS;
        } else
        {
            parent.movingToNextCell = true;
            parent.targetCellPos = transform.forward * cellDistance;
            return NodeState.RUNNING;
        }
    }
}
