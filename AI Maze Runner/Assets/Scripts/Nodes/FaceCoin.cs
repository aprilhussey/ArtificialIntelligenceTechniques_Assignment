using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCoin : Node
{
    private BehaviourTreeScript parent;
    private Transform transform;

    public FaceCoin(BehaviourTreeScript parent, Transform transform)
    {
        this.parent = parent;
        this.transform = transform;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Face Coin");
        transform.rotation = Quaternion.LookRotation(parent.coinDirection);
        return NodeState.SUCCESS;
    }
}
