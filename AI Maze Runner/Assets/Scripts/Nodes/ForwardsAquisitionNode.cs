using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardsAquisitionNode : Node
{
    private Transform transform;
    private float coinVisibilityDistance;
    private int coinMask;
    private int obstacleMask;
    private float fieldOfView;
    private BehaviourTreeScript parent;
    private GameObject targetCoin;
    private bool coinFirstRun;
    private Rigidbody rigidBody;
    private float moveSpeed;

    public ForwardsAquisitionNode(Transform transform, float coinVisibilityDistance, int coinMask, int obstacleMask, float fieldOfView, BehaviourTreeScript parent, Rigidbody rigidBody, float moveSpeed)
    {
        this.transform = transform;
        this.coinVisibilityDistance = coinVisibilityDistance;
        this.coinMask = coinMask;
        this.obstacleMask = obstacleMask;
        this.fieldOfView = fieldOfView;
        this.parent = parent;
        this.rigidBody = rigidBody;
        this.moveSpeed = moveSpeed;
        coinFirstRun = true;
    }

    bool CoinCollected()
    {
        return parent.targetCoin == null;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Evaluate ForwardsAquire");
        // Ensure target coin exists
        if (coinFirstRun && parent.targetCoin == null)
        {
            return NodeState.FAILURE;
        }

        // Make the NPC move forward at the defined speed
        if (CoinCollected())
        {
            coinFirstRun = true;
            return NodeState.SUCCESS;
        } else
        {
            coinFirstRun = false;
            this.rigidBody.velocity = this.transform.forward * this.moveSpeed;
            return NodeState.RUNNING;
        }
    }
}
