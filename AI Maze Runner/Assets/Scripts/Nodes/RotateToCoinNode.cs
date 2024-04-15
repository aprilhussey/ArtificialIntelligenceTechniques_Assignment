using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCoinNode : Node
{
    private Transform transform;
    private float coinVisibilityDistance;
    private int coinMask;
    private int obstacleMask;
    private float fieldOfView;
    private BehaviourTreeScript parent;

    public RotateToCoinNode(Transform transform, float coinVisibilityDistance, int coinMask, int obstacleMask, float fieldOfView, BehaviourTreeScript parent)
    {
        this.transform = transform;
        this.coinVisibilityDistance = coinVisibilityDistance;
        this.coinMask = coinMask;
        this.obstacleMask = obstacleMask;
        this.fieldOfView = fieldOfView;
        this.parent = parent;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Evaluate RotateToCoin");
        // check if parent does not have targetCoin
        if (parent.targetCoin == null)
        {
            return NodeState.FAILURE;
        }

        Vector3 targetDir = parent.targetCoin.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(targetDir);
        return NodeState.SUCCESS;
    }
}
