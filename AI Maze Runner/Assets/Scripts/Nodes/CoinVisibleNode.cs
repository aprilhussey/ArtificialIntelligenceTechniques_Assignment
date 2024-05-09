using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinVisibleNode : Node
{
    private Transform transform;
    private float coinVisibilityDistance;
    private BehaviourTreeScript parent;

    public CoinVisibleNode(Transform transform, float coinVisibilityDistance, BehaviourTreeScript parent)
    {
        this.transform = transform;
        this.coinVisibilityDistance = coinVisibilityDistance;
        this.parent = parent;
    }

    (bool, GameObject) CoinInDirection(Vector3 direction)
    {
        // Return a tuple containing a bool for whether a coin was seen, and a GameObject containing that coin otherwise return null.
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;
        bool raycastResult = Physics.Raycast(ray, out hit, coinVisibilityDistance);

        if (hit.collider != null)
        {
            GameObject hitObject = hit.collider.gameObject;
            (bool isCoin, GameObject possibleCoin) = parent.CheckIfCoinOrParentCoin(hitObject);
            // If raycast hit something and it's a coin.
            return (raycastResult && isCoin, possibleCoin);
        }
        return (raycastResult, null);
    }

    bool CoinVisible()
    {
        // Is there a coin in any direction, use 4 raycasts to check! Max distance raycast stopping for walls.
        // Check Front, Left, Right, Backwards
        List<Vector3> directionsToCheck = new List<Vector3>{ transform.forward, -transform.right, transform.right, -transform.forward };
           
        foreach (Vector3 direction in directionsToCheck)
        {
            (bool coinVisible, GameObject possibleCoin) = CoinInDirection(direction);
            if (coinVisible)
            {
                parent.coinDirection = direction;
                parent.targetCoin = possibleCoin;
                return true;
            }
        }
        return false;
    }

    public override NodeState Evaluate()
    {
        //Debug.Log("Evaluate CoinVisible");
        if (parent.movingToCoin || parent.targetCoin != null)
        {
            // Coin targetted continue
            return NodeState.SUCCESS;
        }
        else if (CoinVisible() && !parent.movingToCoin)
        {
            return NodeState.SUCCESS;
        } else
        {
            return NodeState.FAILURE;
        }
    }
}
