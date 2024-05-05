using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnAround : Node
{
    private Transform transform;

    public TurnAround(Transform transform)
    {
        this.transform = transform;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Turning around");
        // Turn around
        transform.Rotate(0, 180, 0);
        return NodeState.SUCCESS;
    }
}
