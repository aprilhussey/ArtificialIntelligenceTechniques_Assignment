using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnAround : Node
{
    private Transform transform;
    private BehaviourTreeScript parent;

    public TurnAround(BehaviourTreeScript parent, Transform transform)
    {
        this.transform = transform;
        this.parent = parent;
    }

    public override NodeState Evaluate()
    {
        if (!parent.movingToCell && !parent.movingToCoin)
        {
            // Turn around
            transform.Rotate(0, 180, 0);
            return NodeState.SUCCESS;
        } else
        {
            return NodeState.FAILURE;
        }
        
    }
}
