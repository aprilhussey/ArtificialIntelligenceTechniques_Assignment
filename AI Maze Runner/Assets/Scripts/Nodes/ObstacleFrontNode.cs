using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleFrontNode : Node
{
    private Transform transform;
    private float obstacleVisibilityDistance;
    private BehaviourTreeScript parent;

    public ObstacleFrontNode(Transform transform, BehaviourTreeScript parent, float obstacleVisibilityDistance)
    {
        this.transform = transform;
        this.parent = parent;
        this.obstacleVisibilityDistance = obstacleVisibilityDistance;
    }

    bool ObstacleVisible()
    {
        // If a none coin obstacle is directly infront of the NPC less than half the distance of a maze cell's length
        // Use a raycast to find the next object for the ray
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        bool raycastResult = Physics.Raycast(ray, out hit, obstacleVisibilityDistance);

        if (hit.collider != null)
        {
            GameObject hitObject = hit.collider.gameObject;
            (bool isCoin, GameObject possibleCoin) = parent.CheckIfCoinOrParentCoin(hitObject);
            // If raycast hit something and it's not a coin.
            return raycastResult && !isCoin;
        }
        return raycastResult;
    }

    public override NodeState Evaluate()
    {
        //Debug.Log("Evaluate ObstacleFront");
        if (ObstacleVisible())
        {
            Debug.Log("OBSTACLE VISIBLE");
            return NodeState.SUCCESS;
        } else
        {
            return NodeState.FAILURE;
        }
        
    }
}
