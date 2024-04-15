using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinVisibleNode : Node
{
    private Transform transform;
    private float coinVisibilityDistance;
    private int coinMask;
    private int obstacleMask;
    private float fieldOfView;
    private BehaviourTreeScript parent;

    public CoinVisibleNode(Transform transform, float coinVisibilityDistance, int coinMask, int obstacleMask, float fieldOfView, BehaviourTreeScript parent)
    {
        this.transform = transform;
        this.coinVisibilityDistance = coinVisibilityDistance;
        this.coinMask = coinMask;
        this.obstacleMask = obstacleMask;
        this.fieldOfView = fieldOfView;
        this.parent = parent;
    }

    bool CoinVisible()
    {
        // Determine if a coin is visible and if so, set it as the target and return true, else return false.
        // A coin is visible if it is visible without walls in the way for half the coin, use the position of the coin to determine if half is visible from the position of the capsule, this is simplified by using a raycast between the center of both NPC and Coin.
        Collider[] coinsInViewRadius = Physics.OverlapSphere(transform.position, coinVisibilityDistance, coinMask);

        foreach (Collider coinCollider in coinsInViewRadius)
        {
            Vector3 directionToCoin = (coinCollider.transform.position - transform.position).normalized;

            // Check if coin is within field of view
            if (Vector3.Angle(transform.forward, directionToCoin) < fieldOfView / 2)
            {
                float distanceToCoin = Vector3.Distance(transform.position, coinCollider.transform.position);

                // Check if there are obstructions between the NPC and the coin
                if (!Physics.Raycast(transform.position, directionToCoin, distanceToCoin, obstacleMask))
                {
                    if (coinCollider.gameObject.tag != "Coin")
                    {
                        // Assume it's one of the objects (cube, sphere, etc) that make up the coin instead of the gameObject we need.
                        parent.targetCoin = coinCollider.transform.parent.gameObject;
                    }
                    else
                    {
                        parent.targetCoin = coinCollider.gameObject;
                    }
                    return true;
                }
            }
        }
        return false;
    }

    public override NodeState Evaluate()
    {
        //Debug.Log("Evaluate CoinVisible");
        if (CoinVisible())
        {
            Debug.Log("COIN VISIBLE");
            return NodeState.SUCCESS;
        } else
        {
            return NodeState.FAILURE;
        }
    }
}
